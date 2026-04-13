import { FileTextOutlined } from "@ant-design/icons";
import styles from "./DocumentosWorkbench.module.css";

const DOCUMENTS = [
  {
    id: "doc-001",
    title: "Radicado_2026_0413.pdf",
    meta: "PDF · 2.4 MB",
  },
  {
    id: "doc-002",
    title: "Anexo_Soporte_Contrato.png",
    meta: "Imagen · 1.1 MB",
  },
  {
    id: "doc-003",
    title: "Informe_Tecnico_v2.docx",
    meta: "Documento · 820 KB",
  },
];

export function DocumentosList() {
  return (
    <div className={styles.documentList} role="list">
      {DOCUMENTS.map((item) => (
        <article key={item.id} className={styles.documentCard} role="listitem">
          <span className={styles.documentIcon} aria-hidden="true">
            <FileTextOutlined />
          </span>
          <div>
            <p className={styles.documentTitle}>{item.title}</p>
            <p className={styles.documentMeta}>{item.meta}</p>
          </div>
        </article>
      ))}
    </div>
  );
}
