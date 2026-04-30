import { FileTextOutlined } from "@ant-design/icons";
import styles from "./DocumentosWorkbench.module.css";

export type DocumentoWorkbenchItem = {
  id: string;
  title: string;
  meta: string;
  kind: "pdf" | "image" | "doc";
};

export type DocumentosListProps = {
  items: DocumentoWorkbenchItem[];
  selectedId?: string | null;
  onSelect?: (doc: DocumentoWorkbenchItem) => void;
};

export function DocumentosList({ items, selectedId, onSelect }: DocumentosListProps) {
  return (
    <div className={styles.documentList} role="list">
      {items.map((item) => (
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

