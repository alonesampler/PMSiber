type Props = {
  value: string;
  onChange: (value: string) => void;
};

const ProjectSearch = ({ value, onChange }: Props) => {
  return (
    <div className="row">
      <input
        type="text"
        className="input"
        placeholder="Поиск по названию, заказчику или исполнителю..."
        value={value}
        onChange={(e) => onChange(e.target.value)}
        style={{ flex: 1 }}
      />
      {value && (
        <button 
          className="btn"
          onClick={() => onChange("")}
        >
          Очистить
        </button>
      )}
    </div>
  );
};

export default ProjectSearch;