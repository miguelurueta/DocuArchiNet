import { DeleteOutlined, FileTextOutlined } from "@ant-design/icons";
import { AppButton } from "../../../../app/Components/UI/AppButton";
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
    href: "/demo/Radicado_2026_0413.pdf",
  },
  {
    id: "doc-002",
    title: "20260410DiagnosticoCCV.pdf",
    meta: "PDF · 4.2 MB",
    kind: "pdf",
    href: "/demo/20260410DiagnosticoCCV.pdf",
  },
  {
    id: "doc-003",
    title: "Anexo_Soporte_Contrato.png",
    meta: "Imagen · 1.1 MB",
    kind: "image",
    href: null,
  },
  {
    id: "doc-004",
    title: "Informe_Tecnico_v2.docx",
    meta: "Documento · 820 KB",
    kind: "doc",
    href: null,
  },
];

export type DocumentosListProps = {
  items?: DocumentoWorkbenchItem[];
  selectedId?: string | null;
  onSelect?: (doc: DocumentoWorkbenchItem) => void;
  onDelete?: (doc: DocumentoWorkbenchItem) => void;
};

export function DocumentosList({
  items = DOCUMENTS,
  selectedId,
  onSelect,
  onDelete,
}: DocumentosListProps) {
  return (
    <div className={styles.documentList} role="list">
      {items.map((item) => (
        <div
          key={item.id}
          role="listitem"
          className={styles.documentRow}
          data-selected={item.id === selectedId}
        >
          <button
            type="button"
            className={styles.documentCard}
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

          <AppButton
            variant="ghost"
            size="sm"
            iconOnly
            aria-label={`Eliminar ${item.title}`}
            className={styles.documentDelete}
            icon={<DeleteOutlined />}
            onClick={() => onDelete?.(item)}
          />
        </div>
      ))}
    </div>
  );
}

