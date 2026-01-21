import axios from 'axios';
import type { EmployeeResponseDto, EmployeeParamsDto } from '../types/employee.types';

const api = axios.create({
  baseURL: 'http://localhost:5000/api',
});

export const EmployeesApi = {
  getAll(): Promise<EmployeeResponseDto[]> {
    return api.get('/employees').then(r => r.data);
  },

  search(query: string): Promise<EmployeeResponseDto[]> {
    return api.get('/employees/search', { params: { query } }).then(r => r.data);
  },

  create(dto: EmployeeParamsDto): Promise<EmployeeResponseDto> {
    return api.post('/employees', dto).then(r => r.data);
  },

  update(id: string, dto: EmployeeParamsDto): Promise<void> {
    return api.put(`/employees/${id}`, dto);
  },

  delete(id: string): Promise<void> {
    return api.delete(`/employees/${id}`);
  }
};