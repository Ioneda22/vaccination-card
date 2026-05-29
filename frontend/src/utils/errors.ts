import { AxiosError } from "axios";

// Extrai uma mensagem amigável a partir de um erro do Axios,
// entendendo o formato ProblemDetails (RFC 7807) retornado pela API.
export function getApiErrorMessage(
  error: unknown,
  fallback = "Ocorreu um erro inesperado. Tente novamente.",
): string {
  if (error instanceof AxiosError) {
    const data = error.response?.data as
      | {
          title?: string;
          detail?: string;
          errors?: Record<string, string[]>;
        }
      | string
      | undefined;

    if (typeof data === "string" && data.trim()) {
      return data;
    }

    if (data && typeof data === "object") {
      // ValidationProblemDetails: dicionário de erros por campo.
      if (data.errors && typeof data.errors === "object") {
        const messages = Object.values(data.errors).flat();
        if (messages.length > 0) {
          return messages.join(" ");
        }
      }
      if (data.detail) return data.detail;
      if (data.title) return data.title;
    }

    if (error.code === "ERR_NETWORK") {
      return "Não foi possível conectar à API. Verifique se o backend está em execução.";
    }

    if (error.message) return error.message;
  }

  return fallback;
}
