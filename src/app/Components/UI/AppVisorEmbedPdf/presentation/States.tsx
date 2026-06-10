import { ArrowUpOutlined, FileTextOutlined } from "@ant-design/icons";

import styles from "../styles/AppVisorEmbedPdf.module.css";

export function EngineLoadingState() {
  return <div className={styles.center}>Cargando motor PDF…</div>;
}

export function DocumentLoadingState() {
  return <div className={styles.center}>Cargando documento…</div>;
}

type EmptyStateProps = {
  onDocumentHintRequest?: () => void;
};

export function EmptyState({ onDocumentHintRequest }: EmptyStateProps) {
  return (
    <div className={styles.center}>
      <div className={styles.emptyDocumentState}>
        <button
          type="button"
          className={styles.emptyDocumentIcon}
          aria-label="Resaltar listado de documentos"
          onClick={onDocumentHintRequest}
        >
          <FileTextOutlined />
          <ArrowUpOutlined className={styles.emptyDocumentIconBadge} />
        </button>
        <span className={styles.emptyDocumentTitle}>Selecciona un documento</span>
        <span className={styles.emptyDocumentDescription}>
          Elige un archivo del listado lateral derecho de documentos para visualizarlo aqui.
        </span>
      </div>
    </div>
  );
}

export function ErrorState() {
  return (
    <div className={styles.center}>
      <span className={styles.muted}>No fue posible cargar el documento</span>
    </div>
  );
}

