import api from "./api";
import type {
  CreatedResponse,
  CreatePersonRequest,
  CreateVaccinationRecordRequest,
  CreateVaccineRequest,
  LoginRequest,
  LoginResponse,
  Person,
  Vaccine,
  VaccinationCard,
} from "../types";

// Camada de serviços: funções tipadas para cada endpoint da API.

export const authService = {
  login: async (payload: LoginRequest): Promise<LoginResponse> => {
    const { data } = await api.post<LoginResponse>("/auth/login", payload);
    return data;
  },
};

export const vaccinesService = {
  getAll: async (): Promise<Vaccine[]> => {
    const { data } = await api.get<Vaccine[]>("/vaccines");
    return data;
  },
  create: async (payload: CreateVaccineRequest): Promise<CreatedResponse> => {
    const { data } = await api.post<CreatedResponse>("/vaccines", payload);
    return data;
  },
};

export const peopleService = {
  getAll: async (): Promise<Person[]> => {
    const { data } = await api.get<Person[]>("/people");
    return data;
  },
  create: async (payload: CreatePersonRequest): Promise<CreatedResponse> => {
    const { data } = await api.post<CreatedResponse>("/people", payload);
    return data;
  },
  remove: async (personId: string): Promise<void> => {
    await api.delete(`/people/${personId}`);
  },
  getVaccinationCard: async (personId: string): Promise<VaccinationCard> => {
    const { data } = await api.get<VaccinationCard>(
      `/people/${personId}/vaccination-card`,
    );
    return data;
  },
  addVaccinationRecord: async (
    personId: string,
    payload: CreateVaccinationRecordRequest,
  ): Promise<CreatedResponse> => {
    const { data } = await api.post<CreatedResponse>(
      `/people/${personId}/vaccination-records`,
      payload,
    );
    return data;
  },
  removeVaccinationRecord: async (
    personId: string,
    recordId: string,
  ): Promise<void> => {
    await api.delete(`/people/${personId}/vaccination-records/${recordId}`);
  },
};
