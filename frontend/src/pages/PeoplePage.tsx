import { useCallback, useEffect, useState } from "react";
import { useForm } from "react-hook-form";
import { useNavigate } from "react-router-dom";
import { peopleService } from "../api/services";
import { getApiErrorMessage } from "../utils/errors";
import type { CreatePersonRequest, Person } from "../types";
import Modal from "../components/Modal";
import Spinner from "../components/Spinner";

export default function PeoplePage() {
  const navigate = useNavigate();

  const [people, setPeople] = useState<Person[]>([]);
  const [loading, setLoading] = useState(true);
  const [loadError, setLoadError] = useState<string | null>(null);

  const [isModalOpen, setIsModalOpen] = useState(false);
  const [formError, setFormError] = useState<string | null>(null);
  const [deletingId, setDeletingId] = useState<string | null>(null);

  const {
    register,
    handleSubmit,
    reset,
    formState: { errors, isSubmitting },
  } = useForm<CreatePersonRequest>({
    defaultValues: { name: "", identificationNumber: "" },
  });

  const loadPeople = useCallback(async () => {
    setLoading(true);
    setLoadError(null);
    try {
      setPeople(await peopleService.getAll());
    } catch (error) {
      setLoadError(getApiErrorMessage(error));
    } finally {
      setLoading(false);
    }
  }, []);

  useEffect(() => {
    void loadPeople();
  }, [loadPeople]);

  const openModal = () => {
    setFormError(null);
    reset();
    setIsModalOpen(true);
  };

  const onSubmit = async (data: CreatePersonRequest) => {
    setFormError(null);
    try {
      await peopleService.create(data);
      setIsModalOpen(false);
      await loadPeople();
    } catch (error) {
      setFormError(getApiErrorMessage(error));
    }
  };

  const handleDelete = async (person: Person) => {
    const confirmed = window.confirm(
      `Remover "${person.name}"? Isto exclui também o cartão de vacinação e todos os registros associados.`,
    );
    if (!confirmed) return;

    setDeletingId(person.id);
    try {
      await peopleService.remove(person.id);
      setPeople((current) => current.filter((item) => item.id !== person.id));
    } catch (error) {
      alert(getApiErrorMessage(error));
    } finally {
      setDeletingId(null);
    }
  };

  return (
    <div>
      {/* Cabeçalho */}
      <div className="mb-6 flex flex-col gap-3 sm:flex-row sm:items-center sm:justify-between">
        <div>
          <h1 className="text-2xl font-semibold text-slate-800">Pessoas</h1>
          <p className="text-sm text-slate-500">
            Gerencie as pessoas e acesse seus cartões de vacinação.
          </p>
        </div>
        <button
          type="button"
          onClick={openModal}
          className="inline-flex items-center justify-center gap-2 rounded-lg bg-brand-600 px-4 py-2 text-sm font-semibold text-white transition hover:bg-brand-700"
        >
          + Nova pessoa
        </button>
      </div>

      {/* Card de resumo */}
      <div className="mb-6 grid grid-cols-1 gap-4 sm:grid-cols-3">
        <div className="rounded-xl border border-slate-200 bg-white p-4">
          <p className="text-xs uppercase tracking-wide text-slate-400">
            Total de pessoas
          </p>
          <p className="mt-1 text-2xl font-semibold text-slate-800">
            {loading ? "—" : people.length}
          </p>
        </div>
      </div>

      {/* Tabela */}
      <div className="overflow-hidden rounded-xl border border-slate-200 bg-white">
        {loading ? (
          <Spinner label="Carregando pessoas..." />
        ) : loadError ? (
          <div className="flex flex-col items-center gap-3 py-10 text-center">
            <p className="text-sm text-red-600">{loadError}</p>
            <button
              type="button"
              onClick={() => void loadPeople()}
              className="rounded-lg border border-slate-200 px-3 py-1.5 text-sm text-slate-600 hover:bg-slate-50"
            >
              Tentar novamente
            </button>
          </div>
        ) : people.length === 0 ? (
          <div className="py-12 text-center text-sm text-slate-500">
            Nenhuma pessoa cadastrada ainda.
          </div>
        ) : (
          <div className="overflow-x-auto">
            <table className="min-w-full divide-y divide-slate-200 text-sm">
              <thead className="bg-slate-50 text-left text-xs uppercase tracking-wide text-slate-500">
                <tr>
                  <th className="px-4 py-3 font-medium">Nome</th>
                  <th className="px-4 py-3 font-medium">Nº de identificação</th>
                  <th className="px-4 py-3 text-right font-medium">Ações</th>
                </tr>
              </thead>
              <tbody className="divide-y divide-slate-100">
                {people.map((person) => (
                  <tr key={person.id} className="hover:bg-slate-50">
                    <td className="px-4 py-3 font-medium text-slate-800">
                      {person.name}
                    </td>
                    <td className="px-4 py-3 text-slate-600">
                      {person.identificationNumber}
                    </td>
                    <td className="px-4 py-3">
                      <div className="flex justify-end gap-2">
                        <button
                          type="button"
                          onClick={() => navigate(`/people/${person.id}`)}
                          className="rounded-lg bg-brand-50 px-3 py-1.5 text-xs font-semibold text-brand-700 transition hover:bg-brand-100"
                        >
                          Ver cartão
                        </button>
                        <button
                          type="button"
                          onClick={() => void handleDelete(person)}
                          disabled={deletingId === person.id}
                          className="rounded-lg border border-slate-200 px-3 py-1.5 text-xs font-medium text-slate-600 transition hover:border-red-200 hover:bg-red-50 hover:text-red-600 disabled:opacity-50"
                        >
                          {deletingId === person.id ? "Removendo..." : "Remover"}
                        </button>
                      </div>
                    </td>
                  </tr>
                ))}
              </tbody>
            </table>
          </div>
        )}
      </div>

      {/* Modal: nova pessoa */}
      <Modal
        open={isModalOpen}
        title="Nova pessoa"
        onClose={() => setIsModalOpen(false)}
      >
        <form onSubmit={handleSubmit(onSubmit)} className="space-y-4">
          {formError && (
            <div className="rounded-lg bg-red-50 px-3 py-2 text-sm text-red-700">
              {formError}
            </div>
          )}

          <div>
            <label htmlFor="name" className="mb-1 block text-sm font-medium text-slate-700">
              Nome
            </label>
            <input
              id="name"
              type="text"
              className="w-full rounded-lg border border-slate-300 px-3 py-2 text-sm outline-none focus:border-brand-500 focus:ring-2 focus:ring-brand-100"
              placeholder="Maria Silva"
              {...register("name", { required: "Informe o nome." })}
            />
            {errors.name && (
              <p className="mt-1 text-xs text-red-600">{errors.name.message}</p>
            )}
          </div>

          <div>
            <label
              htmlFor="identificationNumber"
              className="mb-1 block text-sm font-medium text-slate-700"
            >
              Nº de identificação
            </label>
            <input
              id="identificationNumber"
              type="text"
              className="w-full rounded-lg border border-slate-300 px-3 py-2 text-sm outline-none focus:border-brand-500 focus:ring-2 focus:ring-brand-100"
              placeholder="12345678900"
              {...register("identificationNumber", {
                required: "Informe o número de identificação.",
              })}
            />
            {errors.identificationNumber && (
              <p className="mt-1 text-xs text-red-600">
                {errors.identificationNumber.message}
              </p>
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
