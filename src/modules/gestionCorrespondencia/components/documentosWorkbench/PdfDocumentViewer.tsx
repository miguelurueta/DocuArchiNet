import { useCallback, useMemo, useState } from "react";
import { Document, Page, pdfjs } from "react-pdf";
import styles from "./DocumentosWorkbench.module.css";

pdfjs.GlobalWorkerOptions.workerSrc = new URL(
  "pdfjs-dist/build/pdf.worker.min.mjs",
  import.meta.url,
).toString();

export type PdfDocumentViewerProps = {
  src?: string | null;
  title?: string;
};

export function PdfDocumentViewer({ src, title = "Documento PDF" }: PdfDocumentViewerProps) {
  const [numPages, setNumPages] = useState(0);
  const [pageNumber, setPageNumber] = useState(1);
  const [zoom, setZoom] = useState(1);

  const canPrev = pageNumber > 1;
  const canNext = numPages > 0 && pageNumber < numPages;

  const handleLoadSuccess = useCallback(({ numPages: nextNumPages }: { numPages: number }) => {
    setNumPages(nextNumPages);
    setPageNumber(1);
  }, []);

  const zoomLabel = useMemo(() => `${Math.round(zoom * 100)}%`, [zoom]);

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
    <section className={styles.pdfViewer} aria-label="Visor de documento PDF">
      <header className={styles.pdfToolbar}>
        <div className={styles.pdfToolbarGroup}>
          <button
            type="button"
            className={styles.pdfButton}
            onClick={() => setPageNumber((prev) => Math.max(1, prev - 1))}
            disabled={!canPrev}
          >
            Anterior
          </button>
          <span className={styles.pdfMeta} aria-label="Indicador de pagina">
            Pagina {pageNumber} de {numPages || "?"}
          </span>
          <button
            type="button"
            className={styles.pdfButton}
            onClick={() => setPageNumber((prev) => (numPages ? Math.min(numPages, prev + 1) : prev))}
            disabled={!canNext}
          >
            Siguiente
          </button>
        </div>

        <div className={styles.pdfToolbarGroup}>
          <button
            type="button"
            className={styles.pdfButton}
            onClick={() => setZoom((prev) => Math.max(0.5, Number((prev - 0.1).toFixed(2))))}
            aria-label="Alejar"
          >
            -
          </button>
          <span className={styles.pdfMeta} aria-label="Nivel de zoom">
            {zoomLabel}
          </span>
          <button
            type="button"
            className={styles.pdfButton}
            onClick={() => setZoom((prev) => Math.min(2, Number((prev + 0.1).toFixed(2))))}
            aria-label="Acercar"
          >
            +
          </button>
          <a className={styles.pdfLink} href={src} target="_blank" rel="noreferrer">
            Abrir
          </a>
        </div>
      </header>

      <div className={styles.pdfCanvas} aria-label={title}>
        <Document file={src} onLoadSuccess={handleLoadSuccess} loading="Cargando PDF...">
          <Page pageNumber={pageNumber} scale={zoom} renderTextLayer={false} renderAnnotationLayer />
        </Document>
      </div>
    </section>
  );
}
