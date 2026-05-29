// Tipagens baseadas nos contratos da API do backend (VaccinationCard.API).

// ---------- Autenticação ----------
export interface LoginRequest {
  userName: string;
  password: string;
}

export interface LoginResponse {
  accessToken: string;
  tokenType: string;
}

export interface AuthUser {
  userName: string;
}

// ---------- Vacinas ----------
export interface Vaccine {
  id: string;
  name: string;
}

export interface CreateVaccineRequest {
  name: string;
}

// ---------- Pessoas ----------
export interface Person {
  id: string;
  name: string;
  identificationNumber: string;
}

export interface CreatePersonRequest {
  name: string;
  identificationNumber: string;
}

// ---------- Cartão de vacinação / registros ----------
// Doses aceitas pelo backend (validadas no servidor).
export type VaccineDose = "Single" | "First" | "Second" | "Third" | "Booster";

export const VACCINE_DOSES: VaccineDose[] = [
  "Single",
  "First",
  "Second",
  "Third",
  "Booster",
];

// Rótulos amigáveis para exibição na UI.
export const DOSE_LABELS: Record<VaccineDose, string> = {
  Single: "Dose única",
  First: "1ª dose",
  Second: "2ª dose",
  Third: "3ª dose",
  Booster: "Reforço",
};

export interface VaccinationRecord {
  recordId: string;
  vaccineId: string;
  vaccineName: string;
  dose: string;
  applicationDate: string; // ISO 8601
}

export interface VaccinationCard {
  personId: string;
  personName: string;
  identificationNumber: string;
  vaccinations: VaccinationRecord[];
}

export interface CreateVaccinationRecordRequest {
  vaccineId: string;
  dose: VaccineDose;
  applicationDate: string; // ISO 8601
}

// Resposta padrão dos endpoints de criação: { id }.
export interface CreatedResponse {
  id: string;
}
