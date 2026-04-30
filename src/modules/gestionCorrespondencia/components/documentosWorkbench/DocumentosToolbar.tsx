import { EyeOutlined, FileSearchOutlined, LinkOutlined } from "@ant-design/icons";
import { AppToolbar } from "../../../../app/Components/UI/AppToolbar";

export type DocumentosToolbarProps = {
  className?: string;
};

export function DocumentosToolbar({ className }: DocumentosToolbarProps) {
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
        },
        {
          key: "vincular",
          label: "Vincular",
          size: "sm",
          variant: "ghost",
          icon: <LinkOutlined />,
        },
      ]}
      primaryAction={{
        key: "abrir",
        label: "Abrir documento",
        size: "sm",
        variant: "ghost",
        icon: <EyeOutlined />,
      }}
      actionContent={null}
      collapseBreakpoint="md"
    />
  );
}
