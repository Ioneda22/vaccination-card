import { useCallback, useEffect, useMemo, useState } from "react";
import { useForm } from "react-hook-form";
import { Link, useParams } from "react-router-dom";
import { peopleService, vaccinesService } from "../api/services";
import { getApiErrorMessage } from "../utils/errors";
import {
  DOSE_LABELS,
  VACCINE_DOSES,
  type VaccinationCard,
  type Vaccine,
  type VaccineDose,
} from "../types";
import Spinner from "../components/Spinner";

interface RecordFormValues {
  vaccineId: string;
  dose: VaccineDose;
  applicationDate: string; // yyyy-mm-dd (input type="date")
}

function formatDate(iso: string): string {
  const date = new Date(iso);
  return Number.isNaN(date.getTime())
    ? iso
    : date.toLocaleDateString("pt-BR", { timeZone: "UTC" });
}

function doseLabel(dose: string): string {
  return DOSE_LABELS[dose as VaccineDose] ?? dose;
}

export default function VaccinationCardPage() {
  const { personId = "" } = useParams<{ personId: string }>();
  const today = useMemo(() => new Date().toISOString().slice(0, 10), []);

  const [card, setCard] = useState<VaccinationCard | null>(null);
  const [vaccines, setVaccines] = useState<Vaccine[]>([]);
  const [loading, setLoading] = useState(true);
  const [loadError, setLoadError] = useState<string | null>(null);

  const [formError, setFormError] = useState<string | null>(null);
  const [deletingId, setDeletingId] = useState<string | null>(null);

  const {
    register,
    handleSubmit,
    reset,
    formState: { errors, isSubmitting },
  } = useForm<RecordFormValues>({
    defaultValues: { vaccineId: "", dose: "First", applicationDate: today },
  });

  const loadData = useCallback(async () => {
    setLoading(true);
    setLoadError(null);
    try {
      const [cardData, vaccinesData] = await Promise.all([
        peopleService.getVaccinationCard(personId),
        vaccinesService.getAll(),
      ]);
      setCard(cardData);
      setVaccines(vaccinesData);
    } catch (error) {
      setLoadError(getApiErrorMessage(error));
    } finally {
      setLoading(false);
    }
  }, [personId]);

  useEffect(() => {
    void loadData();
  }, [loadData]);

  const onSubmit = async (data: RecordFormValues) => {
    setFormError(null);
    try {
      await peopleService.addVaccinationRecord(personId, {
        vaccineId: data.vaccineId,
        dose: data.dose,
        // Converte a data (yyyy-mm-dd) para ISO 8601 esperado pela API.
        applicationDate: new Date(`${data.applicationDate}T00:00:00Z`).toISOString(),
      });
      reset({ vaccineId: "", dose: "First", applicationDate: today });
      await loadData();
    } catch (error) {
      setFormError(getApiErrorMessage(error));
    }
  };

  const handleDeleteRecord = async (recordId: string) => {
    if (!window.confirm("Excluir este registro de vacinação?")) return;

    setDeletingId(recordId);
    try {
      await peopleService.removeVaccinationRecord(personId, recordId);
      setCard((current) =>
        current
          ? {
              ...current,
              vaccinations: current.vaccinations.filter(
                (record) => record.recordId !== recordId,
              ),
            }
          : current,
      );
    } catch (error) {
      alert(getApiErrorMessage(error));
    } finally {
      setDeletingId(null);
    }
  };

  if (loading) {
    return <Spinner label="Carregando cartão de vacinação..." />;
  }

  if (loadError || !card) {
    return (
      <div className="flex flex-col items-center gap-3 rounded-xl border border-slate-200 bg-white py-12 text-center">
        <p className="text-sm text-red-600">
          {loadError ?? "Cartão não encontrado."}
        </p>
        <Link
          to="/"
          className="rounded-lg border border-slate-200 px-3 py-1.5 text-sm text-slate-600 hover:bg-slate-50"
        >
          Voltar para o dashboard
        </Link>
      </div>
    );
  }

  return (
    <div>
      <Link
        to="/"
        className="mb-4 inline-flex items-center gap-1 text-sm text-slate-500 transition hover:text-slate-700"
      >
        ← Voltar
      </Link>

      {/* Dados da pessoa */}
      <div className="mb-6 rounded-xl border border-slate-200 bg-white p-5">
        <h1 className="text-2xl font-semibold text-slate-800">{card.personName}</h1>
        <p className="text-sm text-slate-500">
          Nº de identificação:{" "}
          <span className="font-medium text-slate-700">
            {card.identificationNumber}
          </span>
        </p>
        <p className="mt-1 text-sm text-slate-500">
          Total de vacinas registradas:{" "}
          <span className="font-medium text-slate-700">
            {card.vaccinations.length}
          </span>
        </p>
      </div>

      <div className="grid grid-cols-1 gap-6 lg:grid-cols-3">
        {/* Tabela de vacinações */}
        <div className="lg:col-span-2">
          <div className="overflow-hidden rounded-xl border border-slate-200 bg-white">
            <div className="border-b border-slate-200 px-5 py-3">
              <h2 className="font-semibold text-slate-800">Vacinações registradas</h2>
            </div>
            {card.vaccinations.length === 0 ? (
              <div className="py-12 text-center text-sm text-slate-500">
                Nenhuma vacinação registrada ainda.
              </div>
            ) : (
              <div className="overflow-x-auto">
                <table className="min-w-full divide-y divide-slate-200 text-sm">
                  <thead className="bg-slate-50 text-left text-xs uppercase tracking-wide text-slate-500">
                    <tr>
                      <th className="px-4 py-3 font-medium">Vacina</th>
                      <th className="px-4 py-3 font-medium">Dose</th>
                      <th className="px-4 py-3 font-medium">Aplicação</th>
                      <th className="px-4 py-3 text-right font-medium">Ações</th>
                    </tr>
                  </thead>
                  <tbody className="divide-y divide-slate-100">
                    {card.vaccinations.map((record) => (
                      <tr key={record.recordId} className="hover:bg-slate-50">
                        <td className="px-4 py-3 font-medium text-slate-800">
                          {record.vaccineName}
                        </td>
                        <td className="px-4 py-3">
                          <span className="rounded-full bg-brand-50 px-2.5 py-0.5 text-xs font-medium text-brand-700">
                            {doseLabel(record.dose)}
                          </span>
                        </td>
                        <td className="px-4 py-3 text-slate-600">
                          {formatDate(record.applicationDate)}
                        </td>
                        <td className="px-4 py-3 text-right">
                          <button
                            type="button"
                            onClick={() => void handleDeleteRecord(record.recordId)}
                            disabled={deletingId === record.recordId}
                            className="rounded-lg border border-slate-200 px-3 py-1.5 text-xs font-medium text-slate-600 transition hover:border-red-200 hover:bg-red-50 hover:text-red-600 disabled:opacity-50"
                          >
                            {deletingId === record.recordId ? "Excluindo..." : "Excluir"}
                          </button>
                        </td>
                      </tr>
                    ))}
                  </tbody>
                </table>
              </div>
            )}
          </div>
        </div>

        {/* Formulário: nova vacinação */}
        <div className="lg:col-span-1">
          <div className="rounded-xl border border-slate-200 bg-white p-5">
            <h2 className="mb-4 font-semibold text-slate-800">Registrar vacinação</h2>

            {vaccines.length === 0 ? (
              <div className="rounded-lg bg-amber-50 px-3 py-2 text-sm text-amber-700">
                Cadastre uma vacina primeiro na aba{" "}
                <Link to="/vaccines" className="font-medium underline">
                  Vacinas
                </Link>
                .
              </div>
            ) : (
              <form onSubmit={handleSubmit(onSubmit)} className="space-y-4">
                {formError && (
                  <div className="rounded-lg bg-red-50 px-3 py-2 text-sm text-red-700">
                    {formError}
                  </div>
                )}

                <div>
                  <label htmlFor="vaccineId" className="mb-1 block text-sm font-medium text-slate-700">
                    Vacina
                  </label>
                  <select
                    id="vaccineId"
                    className="w-full rounded-lg border border-slate-300 bg-white px-3 py-2 text-sm outline-none focus:border-brand-500 focus:ring-2 focus:ring-brand-100"
                    {...register("vaccineId", { required: "Selecione a vacina." })}
                  >
                    <option value="">Selecione...</option>
                    {vaccines.map((vaccine) => (
                      <option key={vaccine.id} value={vaccine.id}>
                        {vaccine.name}
                      </option>
                    ))}
                  </select>
                  {errors.vaccineId && (
                    <p className="mt-1 text-xs text-red-600">{errors.vaccineId.message}</p>
                  )}
                </div>

                <div>
                  <label htmlFor="dose" className="mb-1 block text-sm font-medium text-slate-700">
                    Dose
                  </label>
                  <select
                    id="dose"
                    className="w-full rounded-lg border border-slate-300 bg-white px-3 py-2 text-sm outline-none focus:border-brand-500 focus:ring-2 focus:ring-brand-100"
                    {...register("dose", { required: "Selecione a dose." })}
                  >
                    {VACCINE_DOSES.map((dose) => (
                      <option key={dose} value={dose}>
                        {DOSE_LABELS[dose]}
                      </option>
                    ))}
                  </select>
                  {errors.dose && (
                    <p className="mt-1 text-xs text-red-600">{errors.dose.message}</p>
                  )}
                </div>

                <div>
                  <label
                    htmlFor="applicationDate"
                    className="mb-1 block text-sm font-medium text-slate-700"
                  >
                    Data de aplicação
                  </label>
                  <input
                    id="applicationDate"
                    type="date"
                    max={today}
                    className="w-full rounded-lg border border-slate-300 px-3 py-2 text-sm outline-none focus:border-brand-500 focus:ring-2 focus:ring-brand-100"
                    {...register("applicationDate", {
                      required: "Informe a data de aplicação.",
                    })}
                  />
                  {errors.applicationDate && (
                    <p className="mt-1 text-xs text-red-600">
                      {errors.applicationDate.message}
                    </p>
                  )}
                </div>

                <button
                  type="submit"
                  disabled={isSubmitting}
                  className="w-full rounded-lg bg-brand-600 px-4 py-2 text-sm font-semibold text-white transition hover:bg-brand-700 disabled:opacity-60"
                >
                  {isSubmitting ? "Registrando..." : "Registrar"}
                </button>
              </form>
            )}
          </div>
        </div>
      </div>
    </div>
  );
}
