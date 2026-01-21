export interface EmployeeResponseDto {
  id: string;
  firstName: string;
  lastName: string;
  middleName?: string;
  email: string;
}

export interface EmployeeParamsDto {
  firstName: string;
  lastName: string;
  middleName?: string;
  email: string;
}
