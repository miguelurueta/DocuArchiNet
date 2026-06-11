import { useRef } from "react";
import {
  AppDigitalizador,
  type AppDigitalizadorHandle,
} from "../../../../app/Components/UI/AppDigitalizador";
import { AppModal } from "../../../../app/Components/UI/AppModal";
import type { DigitalizacionDocumentalProps } from "../../types/digitalizacion.types";

const buildTitle = (modo?: string) =>
  modo === "adjuntar" ? "Adjuntar digitalizacion" : "Digitalizar documento";

export function DigitalizacionDocumentalModal({
  open,
  context,
  scannerClient,
  apiClient,
  onClose,
  onCompleted,
  onError,
}: DigitalizacionDocumentalProps) {
  const digitalizadorRef = useRef<AppDigitalizadorHandle>(null);
  const title = context?.titulo ?? buildTitle(context?.modo);

  return (
    <AppModal
      open={open}
      title={title}
      width={980}
      onClose={() => digitalizadorRef.current?.close()}
      hideFooter
    >
      <div data-testid="digitalizacion-modal">
        <AppDigitalizador
          ref={digitalizadorRef}
          active={open}
          context={context}
          scannerClient={scannerClient}
          apiClient={apiClient}
          disposeOnClose
          onClose={onClose}
          onCompleted={onCompleted}
          onError={onError}
        />
      </div>
    </AppModal>
  );
}
