import type { ReactNode } from "react";
import {
  Group as PanelGroup,
  Panel,
  Separator as PanelResizeHandle,
} from "react-resizable-panels";
import styles from "./GestionWorkbenchParallelTabs.module.css";

export type GestionWorkbenchParallelTabsProps = {
  gestion: ReactNode;
  documentos: ReactNode;
  className?: string;
};

const joinClasses = (...values: Array<string | false | null | undefined>) =>
  values.filter(Boolean).join(" ");

export function GestionWorkbenchParallelTabs({
  gestion,
  documentos,
  className,
}: GestionWorkbenchParallelTabsProps) {
  return (
    <section
      className={joinClasses(styles.parallelTabs, className)}
      aria-label="Vista paralela de Gestion y Documentos"
    >
      <PanelGroup orientation="horizontal" className={styles.panelGroup}>
        <Panel
          id="gestion"
          defaultSize={50}
          minSize={35}
          className={styles.panel}
        >
          <div className={styles.panelSurface} role="region" aria-label="Gestion">
            <div className={styles.panelContent}>{gestion}</div>
          </div>
        </Panel>
        <PanelResizeHandle className={styles.resizeHandle} aria-label="Redimensionar paneles">
          <span className={styles.resizeGrip} aria-hidden="true" />
        </PanelResizeHandle>
        <Panel
          id="documentos"
          defaultSize={50}
          minSize={35}
          className={styles.panel}
        >
          <div className={styles.panelSurface} role="region" aria-label="Documentos">
            <div className={joinClasses(styles.panelContent, styles.panelContentFixed)}>
              {documentos}
            </div>
          </div>
        </Panel>
      </PanelGroup>
    </section>
  );
}
