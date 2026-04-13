import styles from "./DocumentosWorkbench.module.css";

export function DocumentosPreview() {
  return (
    <section className={styles.preview} aria-label="Vista previa del documento">
      <header className={styles.previewHeader}>
        <p className={styles.previewTitle}>Vista previa</p>
        <span className={styles.previewMeta}>Pagina 1 de 4</span>
      </header>
      <div className={styles.previewSurface}>
        <div className={styles.previewPlaceholder}>
          <span className={styles.previewHint}>Selecciona un documento para ver el detalle.</span>
        </div>
      </div>
    </section>
  );
}
