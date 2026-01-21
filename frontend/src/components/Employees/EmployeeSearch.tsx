type Props = {
  value: string;
  onChange: (v: string) => void;
  onReset: () => void; // Добавили onReset
};

const EmployeeSearch = ({ value, onChange, onReset }: Props) => {
  return (
    <div className="row">
      <input
        className="input"
        placeholder="Поиск сотрудников..."
        value={value}
        onChange={e => onChange(e.target.value)}
        style={{ flex: 1 }}
      />
      {value && (
        <button className="btn" onClick={onReset}>
          Сбросить
        </button>
      )}
    </div>
  );
};

export default EmployeeSearch;