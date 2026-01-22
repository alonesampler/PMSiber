import { useEffect, useState } from 'react';
import { EmployeesApi } from '../../api/employees.api';
import type {
  EmployeeParamsDto,
  EmployeeResponseDto
} from '../../types/employee.types';
import EmployeeCard from './EmployeeCard';
import EmployeeForm from './EmployeeForm';
import EmployeeSearch from './EmployeeSearch';

const EmployeeList = () => {
  const [employees, setEmployees] = useState<EmployeeResponseDto[]>([]);
  const [editing, setEditing] = useState<EmployeeResponseDto | null>(null);
  const [creating, setCreating] = useState(false);

  const [query, setQuery] = useState('');
  const [searchResults, setSearchResults] = useState<EmployeeResponseDto[]>([]);
  const [searchLoading, setSearchLoading] = useState(false);

  const load = async () => {
    const data = await EmployeesApi.getAll();
    setEmployees(data);
  };

  useEffect(() => {
    load();
  }, []);

  useEffect(() => {
    if (!query) {
      setSearchResults([]);
      return;
    }

    setSearchLoading(true);

    const t = setTimeout(() => {
      EmployeesApi.search(query)
        .then(setSearchResults)
        .finally(() => setSearchLoading(false));
    }, 250);

    return () => clearTimeout(t);
  }, [query]);

  const create = async (dto: EmployeeParamsDto) => {
    try {
      await EmployeesApi.create(dto);
      setCreating(false);
      await load();
    } catch (e) {
      console.error(e);
      alert('Ошибка при создании');
    }
  };

  const update = async (dto: EmployeeParamsDto) => {
    if (!editing) return;

    try {
      await EmployeesApi.update(editing.id, dto);
      setEditing(null);
      await load();
    } catch (e) {
      console.error(e);
      alert('Ошибка при обновлении');
    }
  };

  const remove = async (id: string) => {
    if (!confirm('Вы уверены, что хотите удалить сотрудника?')) return;
    
    try {
      await EmployeesApi.delete(id);
      await load();
    } catch (e) {
      console.error(e);
      alert('Ошибка при удалении');
    }
  };

  const shown = query ? searchResults : employees;

  return (
    <div className="container">
      <div className="header">
        <div>
          <h1 className="h1">Сотрудники</h1>
          <div className="sub">Управление сотрудниками компании</div>
        </div>

        <button
          className="btn btnPrimary"
          onClick={() => setCreating(true)}
        >
          + Добавить сотрудника
        </button>
      </div>

      <div className="panel">
        <EmployeeSearch
          value={query}
          onChange={setQuery}
          onReset={() => setQuery('')}
        />

        {query && (
          <div className="muted" style={{ marginTop: 8 }}>
            {searchLoading
              ? 'Поиск...'
              : `Найдено: ${shown.length}`}
          </div>
        )}
      </div>

      <div className="spacer" />

      {creating && (
        <>
          <EmployeeForm
            onSubmit={create}
            onCancel={() => setCreating(false)}
          />
          <div className="spacer" />
        </>
      )}

      {editing && (
        <>
          <EmployeeForm
            initial={editing}
            onSubmit={update}
            onCancel={() => setEditing(null)}
          />
          <div className="spacer" />
        </>
      )}

      {shown.length === 0 ? (
        <div className="panel">
          <div className="muted" style={{ textAlign: 'center', padding: '40px 20px' }}>
            {query ? 'Сотрудники не найдены' : 'Сотрудников пока нет'}
          </div>
        </div>
      ) : (
        <div className="grid">
          {shown.map(e => (
            <EmployeeCard
              key={e.id}
              employee={e}
              onEdit={setEditing}
              onDelete={remove}
            />
          ))}
        </div>
      )}
    </div>
  );
};

export default EmployeeList;