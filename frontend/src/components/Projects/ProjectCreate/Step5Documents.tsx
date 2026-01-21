import { useRef, useState } from "react";
import type { ProjectDraft } from "./Wizard";

type Props = { draft: ProjectDraft; setDraft: (v: ProjectDraft) => void };

const Step5Documents = ({ draft, setDraft }: Props) => {
  const inputRef = useRef<HTMLInputElement | null>(null);
  const [drag, setDrag] = useState(false);

  const addFiles = (files: FileList | null) => {
    if (!files) return;
    const list = Array.from(files);
    setDraft({ ...draft, files: [...draft.files, ...list] });
    if (inputRef.current) inputRef.current.value = "";
  };

  return (
    <div>
      <h2 className="h2">Документы проекта</h2>
      <div className="spacer" />

      <div
        className="card"
        style={{
          padding: 24,
          borderStyle: "dashed",
          borderWidth: 2,
          cursor: "pointer",
          textAlign: "center",
          background: drag ? "#f8fafc" : "white"
        }}
        onClick={() => inputRef.current?.click()}
        onDragOver={(e) => {
          e.preventDefault();
          setDrag(true);
        }}
        onDragLeave={() => setDrag(false)}
        onDrop={(e) => {
          e.preventDefault();
          setDrag(false);
          addFiles(e.dataTransfer.files);
        }}
      >
        <div style={{ fontWeight: 600, marginBottom: 8 }}>
          {drag ? "Отпустите файлы здесь" : "Перетащите файлы или кликните для выбора"}
        </div>
        <div className="muted">Поддерживаемые форматы: PDF, DOC, XLS, TXT</div>
        <div className="muted" style={{ marginTop: 8 }}>
          Выбрано файлов: {draft.files.length}
        </div>
        <input
          ref={inputRef}
          type="file"
          multiple
          style={{ display: "none" }}
          onChange={(e) => addFiles(e.target.files)}
        />
      </div>

      {draft.files.length > 0 && (
        <>
          <div className="spacer" />
          <div className="grid" style={{ gridTemplateColumns: "1fr" }}>
            {draft.files.map((file, idx) => (
              <div key={idx} className="card">
                <div style={{ display: "flex", justifyContent: "space-between", alignItems: "center" }}>
                  <div>
                    <div style={{ fontWeight: "500" }}>{file.name}</div>
                    <div className="muted" style={{ fontSize: "12px" }}>
                      {(file.size / 1024).toFixed(1)} KB
                    </div>
                  </div>
                  <button 
                    className="btn btnDanger"
                    onClick={() => {
                      const newFiles = draft.files.filter((_, i) => i !== idx);
                      setDraft({ ...draft, files: newFiles });
                    }}
                  >
                    Удалить
                  </button>
                </div>
              </div>
            ))}
          </div>
        </>
      )}
    </div>
  );
};

export default Step5Documents;