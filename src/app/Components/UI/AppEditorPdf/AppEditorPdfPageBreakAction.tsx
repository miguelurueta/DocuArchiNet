import { ScissorOutlined } from "@ant-design/icons";
import { AppIconActionButton } from "../AppButton";

type AppEditorPdfPageBreakActionProps = {
  disabled?: boolean;
  onInsertPageBreak: () => boolean;
};

export function AppEditorPdfPageBreakAction({
  disabled = false,
  onInsertPageBreak,
}: AppEditorPdfPageBreakActionProps) {
  return (
    <AppIconActionButton
      variant="secondary"
      size="sm"
      disabled={disabled}
      onClick={() => {
        onInsertPageBreak();
      }}
      icon={<ScissorOutlined />}
      aria-label="Insertar salto de pagina"
      tooltip="Insertar salto de pagina"
      data-testid="app-editor-pdf-page-break-action"
    />
  );
}
