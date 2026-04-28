import { FileTextOutlined } from "@ant-design/icons";
import styles from "./DocumentosWorkbench.module.css";

export type DocumentoWorkbenchItem = {
  id: string;
  title: string;
  meta: string;
  kind: "pdf" | "image" | "doc";
  href?: string | null;
};

export const DOCUMENTS: DocumentoWorkbenchItem[] = [
  {
    id: "doc-001",
    title: "Radicado_2026_0413.pdf",
    meta: "PDF · 2.4 MB",
    kind: "pdf",
    // Placeholder: en integracion real vendra desde backend/almacenamiento.
    href: null,
  },
  {
    id: "doc-002",
    title: "Anexo_Soporte_Contrato.png",
    meta: "Imagen · 1.1 MB",
    kind: "image",
    href: null,
  },
  {
    id: "doc-003",
    title: "Informe_Tecnico_v2.docx",
    meta: "Documento · 820 KB",
    kind: "doc",
    href: null,
  },
];

export type DocumentosListProps = {
  selectedId?: string | null;
  onSelect?: (doc: DocumentoWorkbenchItem) => void;
};

export function DocumentosList({ selectedId, onSelect }: DocumentosListProps) {
  return (
    <div className={styles.documentList} role="list">
      {DOCUMENTS.map((item) => (
        <button
          key={item.id}
          type="button"
          className={styles.documentCard}
          role="listitem"
          data-selected={item.id === selectedId}
          onClick={() => onSelect?.(item)}
        >
          <span className={styles.documentIcon} aria-hidden="true">
            <FileTextOutlined />
          </span>
          <div>
            <p className={styles.documentTitle}>{item.title}</p>
            <p className={styles.documentMeta}>{item.meta}</p>
          </div>
        </button>
      ))}
    </div>
  );
}
