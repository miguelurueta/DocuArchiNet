import type { CSSProperties } from "react";
import styles from "./AppContasoftLoader.module.css";

export type AppContasoftLoaderProps = {
  size?: number | string;
  className?: string;
  label?: string;
};

export function AppContasoftLoader({
  size = 72,
  className,
  label = "Loader Contasoft",
}: AppContasoftLoaderProps) {
  const rootClassName = [styles.root, className].filter(Boolean).join(" ");
  const rootStyle = {
    "--app-contasoft-loader-size": typeof size === "number" ? `${size}px` : size,
  } as CSSProperties;

  return (
    <span className={rootClassName} style={rootStyle} role="img" aria-label={label}>
      <svg
        className={styles.mark}
        viewBox="0 0 80 80"
        focusable="false"
        aria-hidden="true"
      >
        <path
          className={styles.track}
          d="M58 18C50 9.5 35.5 7 25 12.5C13 19 8 30.5 8 40C8 49.5 13 61 25 67.5C35.5 73 50 70.5 58 62"
          pathLength="100"
        />
        <path
          className={styles.fill}
          d="M58 18C50 9.5 35.5 7 25 12.5C13 19 8 30.5 8 40C8 49.5 13 61 25 67.5C35.5 73 50 70.5 58 62"
          pathLength="100"
        />
        <path
          className={styles.innerCut}
          d="M57 29C52 22.5 43.5 19 35 20.5C25 22.5 18.5 30.5 18.5 40C18.5 49.5 25 57.5 35 59.5C43.5 61 52 57.5 57 51"
          pathLength="100"
        />
      </svg>
    </span>
  );
}
