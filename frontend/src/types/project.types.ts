import type { EmployeeResponseDto } from "./employee.types";

// ProjectParamsDto - для основных параметров проекта
export type ProjectParamsDto = {
  name: string;
  customerCompanyName: string;
  executorCompanyName: string;
  startDate: string; // ISO string
  endDate: string;   // ISO string
  priority: number;
};

// ProjectUpsertDto - для создания/обновления проекта
export type ProjectUpsertDto = {
  params: ProjectParamsDto;
  managerId: string;
  employeesIds: string[];
};

// ProjectResponseDto - ответ с сервера
export type ProjectResponseDto = {
  id: string;
  name: string;
  customerCompanyName: string;
  executorCompanyName: string;
  startDate: string;
  endDate: string;
  priority: number;
  manager: EmployeeResponseDto;
  employees: EmployeeResponseDto[];
};

// Для списка проектов (если используется)
export type ProjectListDto = {
  id: string;
  name: string;
  customerCompanyName: string;
  startDate: string;
  endDate: string;
  priority: number;
  managerName: string;
  employeesCount: number;
};

// Документы
export type DocumentResponseDto = {
  id: string;
  fileName: string;
  downloadUrl: string;
  contentType: string;
  fileSize: number;
  uploadDate: string;
};

export type ProjectDraftDto = {
  params: {
    name: string;
    customerCompanyName: string;
    executorCompanyName: string;
    startDate: string; // yyyy-mm-dd
    endDate: string;
    priority: number;
  };
  managerId: string;
  employeesIds: string[];
};

export type ProjectFormData = {
  params: {
    name: string;
    customerCompanyName: string;
    executorCompanyName: string;
    startDate: string; // YYYY-MM-DD формат для input[type="date"]
    endDate: string;   // YYYY-MM-DD формат для input[type="date"]
    priority: number;
  };
  managerId: string;
  employeesIds: string[];
};