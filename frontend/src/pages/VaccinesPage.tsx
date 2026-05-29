import { useCallback, useEffect, useState } from "react";
import { useForm } from "react-hook-form";
import { vaccinesService } from "../api/services";
import { getApiErrorMessage } from "../utils/errors";
import type { CreateVaccineRequest, Vaccine } from "../types";
import Modal from "../components/Modal";
import Spinner from "../components/Spinner";

export default function VaccinesPage() {
  const [vaccines, setVaccines] = useState<Vaccine[]>([]);
  const [loading, setLoading] = useState(true);
  const [loadError, setLoadError] = useState<string | null>(null);

  const [isModalOpen, setIsModalOpen] = useState(false);
  const [formError, setFormError] = useState<string | null>(null);

  const {
    register,
    handleSubmit,
    reset,
    formState: { errors, isSubmitting },
  } = useForm<CreateVaccineRequest>({ defaultValues: { name: "" } });

  const loadVaccines = useCallback(async () => {
    setLoading(true);
    setLoadError(null);
    try {
      setVaccines(await vaccinesService.getAll());
    } catch (error) {
      setLoadError(getApiErrorMessage(error));
    } finally {
      setLoading(false);
    }
  }, []);

  useEffect(() => {
    void loadVaccines();
  }, [loadVaccines]);

  const openModal = () => {
    setFormError(null);
    reset();
    setIsModalOpen(true);
  };

  const onSubmit = async (data: CreateVaccineRequest) => {
    setFormError(null);
    try {
      await vaccinesService.create(data);
      setIsModalOpen(false);
      await loadVaccines();
    } catch (error) {
      setFormError(getApiErrorMessage(error));
    }
  };

  return (
    <div>
      <div className="mb-6 flex flex-col gap-3 sm:flex-row sm:items-center sm:justify-between">
        <div>
          <h1 className="text-2xl font-semibold text-slate-800">Vacinas</h1>
          <p className="text-sm text-slate-500">
            Catálogo de vacinas disponíveis para registro.
          </p>
        </div>
        <button
          type="button"
          onClick={openModal}
          className="inline-flex items-center justify-center gap-2 rounded-lg bg-brand-600 px-4 py-2 text-sm font-semibold text-white transition hover:bg-brand-700"
        >
          + Nova vacina
        </button>
      </div>

      <div className="overflow-hidden rounded-xl border border-slate-200 bg-white">
        {loading ? (
          <Spinner label="Carregando vacinas..." />
        ) : loadError ? (
          <div className="flex flex-col items-center gap-3 py-10 text-center">
            <p className="text-sm text-red-600">{loadError}</p>
            <button
              type="button"
              onClick={() => void loadVaccines()}
              className="rounded-lg border border-slate-200 px-3 py-1.5 text-sm text-slate-600 hover:bg-slate-50"
            >
              Tentar novamente
            </button>
          </div>
        ) : vaccines.length === 0 ? (
          <div className="py-12 text-center text-sm text-slate-500">
            Nenhuma vacina cadastrada ainda.
          </div>
        ) : (
          <div className="overflow-x-auto">
            <table className="min-w-full divide-y divide-slate-200 text-sm">
              <thead className="bg-slate-50 text-left text-xs uppercase tracking-wide text-slate-500">
                <tr>
                  <th className="px-4 py-3 font-medium">Nome</th>
                  <th className="px-4 py-3 font-medium">Identificador</th>
                </tr>
              </thead>
              <tbody className="divide-y divide-slate-100">
                {vaccines.map((vaccine) => (
                  <tr key={vaccine.id} className="hover:bg-slate-50">
                    <td className="px-4 py-3 font-medium text-slate-800">
                      {vaccine.name}
                    </td>
                    <td className="px-4 py-3 font-mono text-xs text-slate-500">
                      {vaccine.id}
                    </td>
                  </tr>
                ))}
              </tbody>
            </table>
          </div>
        )}
      </div>

      <Modal
        open={isModalOpen}
        title="Nova vacina"
        onClose={() => setIsModalOpen(false)}
      >
        <form onSubmit={handleSubmit(onSubmit)} className="space-y-4">
          {formError && (
            <div className="rounded-lg bg-red-50 px-3 py-2 text-sm text-red-700">
              {formError}
            </div>
          )}

          <div>
            <label htmlFor="vaccineName" className="mb-1 block text-sm font-medium text-slate-700">
              Nome da vacina
            </label>
            <input
              id="vaccineName"
              type="text"
              className="w-full rounded-lg border border-slate-300 px-3 py-2 text-sm outline-none focus:border-brand-500 focus:ring-2 focus:ring-brand-100"
              placeholder="COVID-19 (Pfizer)"
              {...register("name", { required: "Informe o nome da vacina." })}
            />
            {errors.name && (
              <p className="mt-1 text-xs text-red-600">{errors.name.message}</p>
            )}
          </div>

          <div className="flex justify-end gap-2 pt-2">
            <button
              type="button"
              onClick={() => setIsModalOpen(false)}
              className="rounded-lg border border-slate-200 px-4 py-2 text-sm font-medium text-slate-600 hover:bg-slate-50"
            >
              Cancelar
            </button>
            <button
              type="submit"
              disabled={isSubmitting}
              className="rounded-lg bg-brand-600 px-4 py-2 text-sm font-semibold text-white transition hover:bg-brand-700 disabled:opacity-60"
            >
              {isSubmitting ? "Salvando..." : "Salvar"}
            </button>
          </div>
        </form>
      </Modal>
    </div>
  );
}
