import { Typography } from "antd";
import styles from "./GestionRespuestaMainTabContent.module.css";

export type GestionRespuestaInfoHeaderMetaItem = {
  label: string;
  value: string;
};

export type GestionRespuestaInfoHeaderProps = {
  title?: string;
  description?: string;
  metadata: GestionRespuestaInfoHeaderMetaItem[];
};

export function GestionRespuestaInfoHeader({
  title,
  description,
  metadata,
}: GestionRespuestaInfoHeaderProps) {
  return (
    <header className={styles.infoHeader}>
      {title ? (
        <Typography.Title level={5} className={styles.infoTitle}>
          {title}
        </Typography.Title>
      ) : null}
      {description ? (
        <Typography.Paragraph className={styles.infoCopy}>
          {description}
        </Typography.Paragraph>
      ) : null}
      <div className={styles.infoMeta}>
        {metadata.map((item) => (
          <span key={item.label} className={styles.infoMetaItem}>
            <strong>{item.label}:</strong>
            <span>{item.value}</span>
          </span>
        ))}
      </div>
    </header>
  );
}
