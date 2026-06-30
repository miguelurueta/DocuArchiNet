import { DigitalizacionDocumentalWorkspace } from "../../../../modules/digitalizacion";
import type {
  DigitalizacionApiClient,
  DigitalizacionContext,
  DigitalizacionDocumentalError,
  DigitalizacionResult,
  DigitalizacionScannerClient,
} from "../../../../modules/digitalizacion";
import styles from "./AppDigitalizador.module.css";

export function AppDigitalizadorWorkspace({
  active = true,
  context,
  apiClient,
  scannerClient,
  onCancel,
  onCompleted,
  onError,
  className,
}: AppDigitalizadorWorkspaceProps) {
  const rootClassName = [styles.embeddedWorkspaceRoot, className].filter(Boolean).join(" ");

  return (
    <div className={rootClassName}>
      <DigitalizacionDocumentalWorkspace
        active={active}
        context={context}
        scannerClient={scannerClient}
        apiClient={apiClient}
        onCancel={onCancel}
        onCompleted={onCompleted}
        onError={onError}
      />
    </div>
  );
}

type AppDigitalizadorWorkspaceProps = {
  active?: boolean;
  context: DigitalizacionContext | null;
  apiClient?: DigitalizacionApiClient;
  scannerClient?: DigitalizacionScannerClient;
  onCancel?: () => void;
  onCompleted: (result: DigitalizacionResult) => void;
  onError?: (error: DigitalizacionDocumentalError) => void;
  className?: string;
};

export type { AppDigitalizadorWorkspaceProps };
