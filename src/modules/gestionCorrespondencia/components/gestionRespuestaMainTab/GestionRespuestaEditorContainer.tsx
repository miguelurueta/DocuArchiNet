import type { ReactNode } from "react";
import { Typography } from "antd";
import styles from "./GestionRespuestaMainTabContent.module.css";

export type GestionRespuestaEditorContainerProps = {
  title: string;
  description: string;
  children: ReactNode;
};

export function GestionRespuestaEditorContainer({
  title,
  description,
  children,
}: GestionRespuestaEditorContainerProps) {
  return (
    <section className={styles.editorContainer} aria-label="Editor principal de respuesta">
      <div>
        <Typography.Title level={5} className={styles.infoTitle}>
          {title}
        </Typography.Title>
        <Typography.Paragraph className={styles.infoCopy}>
          {description}
        </Typography.Paragraph>
      </div>
      <div className={styles.editorSurface}>
        {children}
      </div>
    </section>
  );
}
