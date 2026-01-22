import { useState } from "react";

export type ProjectDraft = {
  name: string;
  startDate: string;
  endDate: string;
  priority: number;

  customerCompanyName: string;
  executorCompanyName: string;

  managerId: string;
  employeesIds: string[];

  files: File[];
};

const initialDraft: ProjectDraft = {
  name: "",
  startDate: "",
  endDate: "",
  priority: 5,

  customerCompanyName: "",
  executorCompanyName: "",

  managerId: "",
  employeesIds: [],

  files: [],
};

export const useWizard = () => {
  const [step, setStep] = useState(1);
  const [draft, setDraft] = useState<ProjectDraft>(initialDraft);

  const next = () => setStep((s) => Math.min(5, s + 1));
  const back = () => setStep((s) => Math.max(1, s - 1));
  const reset = () => {
    setDraft(initialDraft);
    setStep(1);
  };

  return { step, setStep, draft, setDraft, next, back, reset };
};