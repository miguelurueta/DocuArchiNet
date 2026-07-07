import { Alert, Button, Empty, Modal, Space, Spin, Typography, message } from "antd";
import AppTable from "../../../app/Components/UI/AppTable/AppTable";
import { AppTableQueryWrapper } from "../../../app/Components/UI/AppTable/AppTableQueryWrapper";
import { useRadicacionDocumentalContext } from "../hooks/useRadicacionDocumentalContext";
import { useRadicacionPendientesContador } from "../hooks/useRadicacionPendientesContador";
import { useRadicacionPendientesTable } from "../hooks/useRadicacionPendientesTable";
import { useTomarRadicadoPendiente } from "../hooks/useTomarRadicadoPendiente";

interface RadicacionPendientesModalProps {
  open: boolean;
  onClose: () => void;
}

export function RadicacionPendientesModal({
  open,
  onClose,
}: RadicacionPendientesModalProps) {
  const { tieneTramiteDocumentalActivoEstado0 } =
    useRadicacionDocumentalContext();
  const table = useRadicacionPendientesTable(open);
  const contador = useRadicacionPendientesContador(open);
  const tomarPendiente = useTomarRadicadoPendiente({
    onSuccess: () => {
      message.success("Radicado pendiente asignado.");
      onClose();
    },
    onError: (errorMessage) => {
      message.error(errorMessage);
    },
  });

  const loading = table.loading || contador.loading || tomarPendiente.isTaking;

  return (
    <Modal
      title="Radicados pendientes"
      open={open}
      onCancel={onClose}
      footer={[
        <Button key="close" type="primary" onClick={onClose}>
          Cerrar
        </Button>,
      ]}
      width={980}
    >
      <Space direction="vertical" size="middle" style={{ width: "100%" }}>
        {tieneTramiteDocumentalActivoEstado0 ? (
          <Alert
            type="warning"
            showIcon
            message="Ya hay un radicado activo"
            description="Finaliza o envia a pendiente el radicado activo antes de tomar otro."
          />
        ) : null}

        <Typography.Text type="secondary">
          {contador.contador === null
            ? "Pendientes"
            : `Pendientes: ${contador.contador}`}
        </Typography.Text>

        {table.error ? (
          <Alert
            type="error"
            showIcon
            message="No fue posible cargar los pendientes"
            description="Verifica que el API este activo y que la sesion tenga los claims requeridos."
          />
        ) : table.rows.length === 0 && !table.loading ? (
          <Empty
            image={Empty.PRESENTED_IMAGE_SIMPLE}
            description="No hay radicados pendientes disponibles."
          />
        ) : (
          <Spin spinning={loading}>
            <AppTableQueryWrapper
              queryState={table.queryState}
              onQueryChange={table.onQueryChange}
              onRefresh={table.refetch}
              total={table.total}
              loading={loading}
              searchPlaceholder="Buscar radicado, remitente o tramite"
            >
              <AppTable
                rows={table.rows}
                columns={table.columns}
                total={table.total}
                paginationMode="server"
                layoutMode="content"
                responsivePresentation={{ enabled: true }}
                getRowId={(row) => String(row.id ?? row.id_estado_radicado)}
                onActionTriggered={tomarPendiente.tomarDesdeAccion}
                onRowClicked={tomarPendiente.tomarDesdeFila}
                rowClickAffordance
                rowClickTooltip="Tomar pendiente"
              />
            </AppTableQueryWrapper>
          </Spin>
        )}
      </Space>
    </Modal>
  );
}
