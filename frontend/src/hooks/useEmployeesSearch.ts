import { useEffect, useState } from 'react';
import { EmployeesApi } from '../api/employees.api';
import type { EmployeeResponseDto } from '../types/employee.types';

export function useEmployeesSearch() {
  const [query, setQuery] = useState('');
  const [employees, setEmployees] = useState<EmployeeResponseDto[]>([]);
  const [loading, setLoading] = useState(false);

  useEffect(() => {
    const timeout = setTimeout(() => {
      setLoading(true);

      const req = query.trim().length > 0
        ? EmployeesApi.search(query)
        : EmployeesApi.getAll();

      req
        .then(setEmployees)
        .finally(() => setLoading(false));
    }, 300);

    return () => clearTimeout(timeout);
  }, [query]);

  return {
    query,
    setQuery,
    employees,
    loading,
    setEmployees
  };
}
