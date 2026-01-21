import type { EmployeeResponseDto } from "../../types/employee.types";

type Props = {
  employee: EmployeeResponseDto;
  onEdit?: (employee: EmployeeResponseDto) => void;
  onDelete?: (id: string) => void;
};

const EmployeeCard = ({ employee, onEdit, onDelete }: Props) => {
  return (
    <div className="card">
      <h3 className="h3">
        {employee.lastName} {employee.firstName}
        {employee.middleName && ` ${employee.middleName}`}
      </h3>

      <p className="muted">{employee.email}</p>

      <div style={{ display: "flex", gap: "8px", marginTop: "16px" }}>
        <button
          className="btn"
          onClick={() => onEdit?.(employee)}
        >
          Редактировать
        </button>

        <button
          className="btn btnDanger"
          onClick={() => onDelete?.(employee.id)}
        >
          Удалить
        </button>
      </div>
    </div>
  );
};

export default EmployeeCard;