import styles from "./DocumentosWorkbench.module.css";

export type PdfDocumentViewerProps = {
  src?: string | null;
  title?: string;
};

export function PdfDocumentViewer({ src, title = "Documento PDF" }: PdfDocumentViewerProps) {
  if (!src) {
    return (
      <div className={styles.pdfEmpty} role="status" aria-label="Sin documento seleccionado">
        <p className={styles.pdfEmptyTitle}>Sin documento seleccionado</p>
        <p className={styles.pdfEmptyCopy}>
          Selecciona un documento del panel derecho para visualizarlo aqui.
        </p>
      </div>
    );
  }

  return (
    <div className={styles.pdfViewer} aria-label="Visor de documento PDF">
      <object className={styles.pdfObject} data={src} type="application/pdf" aria-label={title}>
        <p className={styles.pdfFallback}>
          No fue posible cargar el visor PDF.{" "}
          <a href={src} target="_blank" rel="noreferrer">
            Abrir documento
          </a>
          .
        </p>
      </object>
    </div>
  );
}

