import type { ReactNode } from "react";
import { useEffect, useId, useRef, useState } from "react";
import styles from "./AppLoadingState.module.css";

export type AppLoadingStateProps = {
  loading: boolean;
  delayMs?: number;
  title?: string;
  message?: string;
  icon?: ReactNode;
  className?: string;
  children?: ReactNode;
};

export function AppLoadingState({
  loading,
  delayMs = 500,
  title = "Cargando…",
  message,
  icon,
  className,
  children,
}: AppLoadingStateProps) {
  const [isVisible, setIsVisible] = useState(false);
  const timeoutRef = useRef<number | null>(null);
  const statusId = useId();

  useEffect(() => {
    if (timeoutRef.current !== null) {
      window.clearTimeout(timeoutRef.current);
      timeoutRef.current = null;
    }

    if (!loading) {
      setIsVisible(false);
      return;
    }

    const ms = Number.isFinite(delayMs) ? delayMs : 0;
    if (ms <= 0) {
      setIsVisible(true);
      return;
    }

    timeoutRef.current = window.setTimeout(() => {
      timeoutRef.current = null;
      setIsVisible(true);
    }, ms);

    return () => {
      if (timeoutRef.current !== null) {
        window.clearTimeout(timeoutRef.current);
        timeoutRef.current = null;
      }
    };
  }, [delayMs, loading]);

  if (!loading) {
    return <>{children}</>;
  }

  if (!isVisible) {
    return null;
  }

  const rootClassName = [styles.root, className].filter(Boolean).join(" ");

  return (
    <section
      className={rootClassName}
      role="status"
      aria-live="polite"
      aria-describedby={message ? statusId : undefined}
    >
      {title ? <h3 className={styles.title}>{title}</h3> : null}
      {message || icon ? (
        <p id={statusId} className={styles.message}>
          {icon ? <span className={styles.icon}>{icon}</span> : null}
          {message ?? ""}
        </p>
      ) : null}
    </section>
  );
}
