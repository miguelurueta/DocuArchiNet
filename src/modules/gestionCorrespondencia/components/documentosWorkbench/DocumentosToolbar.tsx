import { EyeOutlined, FileSearchOutlined, LinkOutlined } from "@ant-design/icons";
import { AppToolbar } from "../../../../app/Components/UI/AppToolbar";

export type DocumentosToolbarProps = {
  className?: string;
  hasDocuments?: boolean;
  canOpenSelected?: boolean;
  onOpenDocuments?: () => void;
  onSearchDocuments?: () => void;
  onLinkDocuments?: () => void;
};

export function DocumentosToolbar({
  className,
  hasDocuments = false,
  canOpenSelected = false,
  onOpenDocuments,
  onSearchDocuments,
  onLinkDocuments,
}: DocumentosToolbarProps) {
  return (
    <AppToolbar
      className={className}
      title="Visualizador de documentos"
      description="Revisa anexos y soportes asociados a la respuesta."
      actions={[
        {
          key: "buscar",
          label: "Buscar",
          size: "sm",
          variant: "ghost",
          icon: <FileSearchOutlined />,
          onClick: onSearchDocuments ?? onOpenDocuments,
          disabled: !hasDocuments,
          tooltip: hasDocuments ? undefined : "No hay documentos para buscar.",
        },
        {
          key: "vincular",
          label: "Vincular",
          size: "sm",
          variant: "ghost",
          icon: <LinkOutlined />,
          onClick: onLinkDocuments,
          disabled: true,
          tooltip: "Acción pendiente de integración.",
        },
      ]}
      primaryAction={{
        key: "abrir",
        label: canOpenSelected ? "Abrir documento" : "Ver documentos",
        size: "sm",
        variant: "ghost",
        icon: <EyeOutlined />,
        onClick: onOpenDocuments,
        disabled: !hasDocuments,
        tooltip: hasDocuments ? undefined : "No hay documentos adjuntos.",
      }}
      actionContent={null}
      collapseBreakpoint="md"
    />
  );
}
