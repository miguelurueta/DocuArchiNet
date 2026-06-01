import { FileSearchOutlined } from "@ant-design/icons";

import styles from "../styles/AppVisorEmbedPdf.module.css";

export function EngineLoadingState() {
  return <div className={styles.center}>Cargando motor PDF…</div>;
}

export function DocumentLoadingState() {
  return <div className={styles.center}>Cargando documento…</div>;
}

export function EmptyState() {
  return (
    <div className={styles.center}>
      <div className={styles.emptyDocumentState}>
        <span className={styles.emptyDocumentIcon} aria-hidden="true">
          <FileSearchOutlined />
        </span>
        <span className={styles.emptyDocumentTitle}>Selecciona un documento</span>
        <span className={styles.emptyDocumentDescription}>
          Elige un archivo del listado lateral de documentos para visualizarlo aqui.
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

