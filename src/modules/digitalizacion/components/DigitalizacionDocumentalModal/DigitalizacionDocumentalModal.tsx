import { AppModal } from "../../../../app/Components/UI/AppModal";
import {
  buildDigitalizacionTitle,
  DigitalizacionDocumentalWorkspace,
} from "../DigitalizacionDocumentalWorkspace";
import type { DigitalizacionDocumentalProps, DigitalizacionResult } from "../../types/digitalizacion.types";

export function DigitalizacionDocumentalModal({
  open,
  context,
  onClose,
  onCompleted,
  ...workspaceProps
}: DigitalizacionDocumentalProps) {
  const title = context?.titulo ?? buildDigitalizacionTitle(context?.modo);

  const handleCompleted = (result: DigitalizacionResult) => {
    onCompleted(result);
    onClose();
  };

  return (
    <AppModal open={open} title={title} width={980} onClose={onClose}>
      <DigitalizacionDocumentalWorkspace
        {...workspaceProps}
        active={open}
        context={context}
        onCancel={onClose}
        onCompleted={handleCompleted}
      />
    </AppModal>
  );
}
