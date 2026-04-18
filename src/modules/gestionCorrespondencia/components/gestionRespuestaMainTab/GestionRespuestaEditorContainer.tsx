import type { ReactNode } from "react";
import styles from "./GestionRespuestaMainTabContent.module.css";

export type GestionRespuestaEditorContainerProps = {
  children: ReactNode;
};

export function GestionRespuestaEditorContainer({
  children,
}: GestionRespuestaEditorContainerProps) {
  return (
    <section className={styles.editorContainer} aria-label="Editor principal de respuesta">
      {children}
    </section>
  );
}
