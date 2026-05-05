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
      <span className={styles.muted}>No hay documento para visualizar</span>
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

