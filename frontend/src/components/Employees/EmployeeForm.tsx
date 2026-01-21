import { useState } from 'react';
import type {
  EmployeeParamsDto,
  EmployeeResponseDto
} from '../../types/employee.types';

type Props = {
  initial?: EmployeeResponseDto | null;
  onSubmit: (dto: EmployeeParamsDto) => void;
  onCancel: () => void;
};

const EmployeeForm = ({ initial, onSubmit, onCancel }: Props) => {
  const [form, setForm] = useState<EmployeeParamsDto>({
    firstName: initial?.firstName ?? '',
    lastName: initial?.lastName ?? '',
    middleName: initial?.middleName ?? '',
    email: initial?.email ?? ''
  });

  const handleSubmit = () => {
    if (!form.firstName.trim() || !form.lastName.trim() || !form.email.trim()) {
      alert('Заполните обязательные поля');
      return;
    }
    onSubmit(form);
  };

  return (
    <div className="panel">
      <h3 className="h3">
        {initial ? 'Редактирование сотрудника' : 'Добавление сотрудника'}
      </h3>
      
      <div className="spacer" />

      <div className="row">
        <div style={{ flex: 1 }}>
          <label className="muted" style={{ display: 'block', marginBottom: '4px' }}>
            Имя *
          </label>
          <input
            className="input"
            placeholder="Введите имя"
            value={form.firstName}
            onChange={e => setForm({ ...form, firstName: e.target.value })}
          />
        </div>
        <div style={{ flex: 1 }}>
          <label className="muted" style={{ display: 'block', marginBottom: '4px' }}>
            Фамилия *
          </label>
          <input
            className="input"
            placeholder="Введите фамилию"
            value={form.lastName}
            onChange={e => setForm({ ...form, lastName: e.target.value })}
          />
        </div>
      </div>

      <div className="spacer" />

      <div className="row">
        <div style={{ flex: 1 }}>
          <label className="muted" style={{ display: 'block', marginBottom: '4px' }}>
            Отчество
          </label>
          <input
            className="input"
            placeholder="Введите отчество"
            value={form.middleName ?? ''}
            onChange={e => setForm({ ...form, middleName: e.target.value })}
          />
        </div>
        <div style={{ flex: 1 }}>
          <label className="muted" style={{ display: 'block', marginBottom: '4px' }}>
            Email *
          </label>
          <input
            className="input"
            placeholder="Введите email"
            type="email"
            value={form.email}
            onChange={e => setForm({ ...form, email: e.target.value })}
          />
        </div>
      </div>

      <div className="spacer" />

      <div className="row">
        <button
          className="btn btnPrimary"
          onClick={handleSubmit}
        >
          {initial ? 'Обновить' : 'Создать'}
        </button>

        <button className="btn" onClick={onCancel}>
          Отмена
        </button>
      </div>
    </div>
  );
};

export default EmployeeForm;