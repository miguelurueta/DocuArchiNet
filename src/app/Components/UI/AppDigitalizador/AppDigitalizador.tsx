import { useMemo } from "react";
import { digitalizacionApiClient } from "../../../../modules/digitalizacion";
import { useAppDigitalizadorProvider } from "./AppDigitalizador.context";
import type { AppDigitalizadorProps } from "./AppDigitalizador.types";
import { AppDigitalizadorWorkspace } from "./AppDigitalizadorWorkspace";
import { useAppDigitalizadorScannerClient } from "./hooks/useAppDigitalizadorScannerClient";
import styles from "./AppDigitalizador.module.css";

const DEFAULT_MODULE = "Digitalizacion";

const getModuleName = (props: Pick<AppDigitalizadorProps, "modulo" | "context">) =>
  props.modulo ?? props.context?.sourceModule ?? DEFAULT_MODULE;

export function AppDigitalizador({
  context,
  onCompleted,
  mode = "page",
  active = true,
  modulo,
  apiClient,
  scannerClient,
  dynamsoft,
  licenciaDynamsoft,
  className,
  onCancel,
  onError,
}: AppDigitalizadorProps) {
  const provider = useAppDigitalizadorProvider();
  const resolvedScannerClient = useAppDigitalizadorScannerClient({
    scannerClient,
    dynamsoft,
    licenciaDynamsoft,
  });
  const moduleName = getModuleName({ modulo, context });
  const resolvedContext = useMemo(
    () =>
      context && !context.sourceModule
        ? {
            ...context,
            sourceModule: moduleName,
          }
        : context,
    [context, moduleName],
  );
  const resolvedApiClient = apiClient ?? provider.apiClient ?? digitalizacionApiClient;
  const missingLicense = Boolean(
    !scannerClient &&
      !licenciaDynamsoft?.trim() &&
      !dynamsoft?.licenseKey?.trim() &&
      !provider.dynamsoft?.licenseKey?.trim(),
  );
  const rootClassName = [styles.root, className].filter(Boolean).join(" ");
  if (mode === "embedded") {
    return (
      <AppDigitalizadorWorkspace
        active={active}
        context={resolvedContext}
        scannerClient={resolvedScannerClient}
        apiClient={resolvedApiClient}
        onCancel={onCancel}
        onCompleted={onCompleted}
        onError={onError}
        className={className}
      />
    );
  }

  return (
    <section
      className={rootClassName}
      aria-label="AppDigitalizador"
      data-module={moduleName}
      data-testid="app-digitalizador"
    >
      <header className={styles.header}>
        <div className={styles.titleGroup}>
          <span className={styles.title}>Digitalizador documental</span>
          <span className={styles.subtitle}>Entrada corporativa de digitalizacion</span>
        </div>
        <span className={styles.moduleBadge}>{moduleName}</span>
      </header>

      {missingLicense ? (
        <div className={styles.warning} role="status">
          Licencia Dynamsoft pendiente. Configure la licencia o el proveedor corporativo.
        </div>
      ) : null}

      <div className={styles.workspaceFrame}>
        <DigitalizacionDocumentalWorkspace
          active={active}
          context={resolvedContext}
          scannerClient={resolvedScannerClient}
          apiClient={resolvedApiClient}
          onCancel={onCancel}
          onCompleted={onCompleted}
          onError={onError}
        />
      </div>
    </section>
  );
}
