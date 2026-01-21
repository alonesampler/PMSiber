import type { ProjectDraft } from "./Wizard";

type Props = { draft: ProjectDraft; setDraft: (v: ProjectDraft) => void };

const Step2Companies = ({ draft, setDraft }: Props) => {
  return (
    <div>
      <h2 className="h2">Информация о компаниях</h2>
      <div className="spacer" />
      <input
        className="input"
        placeholder="Компания-заказчик *"
        value={draft.customerCompanyName}
        onChange={(e) => setDraft({ ...draft, customerCompanyName: e.target.value })}
      />
      <div className="spacer" />
      <input
        className="input"
        placeholder="Компания-исполнитель *"
        value={draft.executorCompanyName}
        onChange={(e) => setDraft({ ...draft, executorCompanyName: e.target.value })}
      />
    </div>
  );
};

export default Step2Companies;