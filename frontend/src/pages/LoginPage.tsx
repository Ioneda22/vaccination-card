import { useState } from "react";
import { useForm } from "react-hook-form";
import { useNavigate } from "react-router-dom";
import { useAuth } from "../context/AuthContext";
import { getApiErrorMessage } from "../utils/errors";
import type { LoginRequest } from "../types";

export default function LoginPage() {
  const { login } = useAuth();
  const navigate = useNavigate();
  const [serverError, setServerError] = useState<string | null>(null);

  const {
    register,
    handleSubmit,
    formState: { errors, isSubmitting },
  } = useForm<LoginRequest>({
    defaultValues: { userName: "", password: "" },
  });

  const onSubmit = async (data: LoginRequest) => {
    setServerError(null);
    try {
      await login(data.userName, data.password);
      navigate("/", { replace: true });
    } catch (error) {
      setServerError(getApiErrorMessage(error, "Falha ao autenticar."));
    }
  };

  return (
    <div className="flex min-h-screen items-center justify-center px-4">
      <div className="w-full max-w-sm">
        <div className="mb-6 text-center">
          <span className="mx-auto mb-3 flex h-12 w-12 items-center justify-center rounded-xl bg-brand-600 text-2xl font-bold text-white">
            V
          </span>
          <h1 className="text-xl font-semibold text-slate-800">
            Cartão de Vacinação
          </h1>
          <p className="text-sm text-slate-500">Entre para acessar o painel</p>
        </div>

        <form
          onSubmit={handleSubmit(onSubmit)}
          className="space-y-4 rounded-xl border border-slate-200 bg-white p-6 shadow-sm"
        >
          {serverError && (
            <div className="rounded-lg bg-red-50 px-3 py-2 text-sm text-red-700">
              {serverError}
            </div>
          )}

          <div>
            <label htmlFor="userName" className="mb-1 block text-sm font-medium text-slate-700">
              Utilizador
            </label>
            <input
              id="userName"
              type="text"
              autoComplete="username"
              className="w-full rounded-lg border border-slate-300 px-3 py-2 text-sm outline-none transition focus:border-brand-500 focus:ring-2 focus:ring-brand-100"
              placeholder="admin"
              {...register("userName", { required: "Informe o utilizador." })}
            />
            {errors.userName && (
              <p className="mt-1 text-xs text-red-600">{errors.userName.message}</p>
            )}
          </div>

          <div>
            <label htmlFor="password" className="mb-1 block text-sm font-medium text-slate-700">
              Senha
            </label>
            <input
              id="password"
              type="password"
              autoComplete="current-password"
              className="w-full rounded-lg border border-slate-300 px-3 py-2 text-sm outline-none transition focus:border-brand-500 focus:ring-2 focus:ring-brand-100"
              placeholder="••••••"
              {...register("password", { required: "Informe a senha." })}
            />
            {errors.password && (
              <p className="mt-1 text-xs text-red-600">{errors.password.message}</p>
            )}
          </div>

          <button
            type="submit"
            disabled={isSubmitting}
            className="w-full rounded-lg bg-brand-600 px-4 py-2 text-sm font-semibold text-white transition hover:bg-brand-700 disabled:cursor-not-allowed disabled:opacity-60"
          >
            {isSubmitting ? "Entrando..." : "Entrar"}
          </button>

          <p className="text-center text-xs text-slate-400">
            Credenciais de demonstração: <span className="font-medium">admin / admin</span>
          </p>
        </form>
      </div>
    </div>
  );
}
