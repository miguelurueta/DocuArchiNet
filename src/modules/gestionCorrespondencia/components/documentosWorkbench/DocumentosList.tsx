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

export type DocumentosListProps = {
  items?: DocumentoWorkbenchItem[];
  selectedId?: string | null;
  onSelect?: (doc: DocumentoWorkbenchItem) => void;
  onDelete?: (doc: DocumentoWorkbenchItem) => void;
};

export function DocumentosList({
  items = [],
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
          <button type="button" className={styles.documentCard} onClick={() => onSelect?.(item)}>
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

