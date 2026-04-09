import { Alert, Card, Divider, Space, Tag, Typography } from "antd";

export default function GestionRespuesta() {
  return (
    <Space orientation="vertical" size={16} style={{ display: "flex" }}>
      <Space size={8} wrap>
        <Tag color="blue">Bandeja activa</Tag>
        <Tag>Retorno contextual</Tag>
      </Space>

      <Typography.Title level={4} style={{ margin: 0 }}>
        Gestion de respuesta
      </Typography.Title>
      <Typography.Paragraph style={{ margin: 0 }}>
        Esta vista secundaria funciona como placeholder del flujo de respuesta.
        Mantiene el contexto de la bandeja visible y deja el retorno al listado
        en la accion principal del shell, sin acoplar esta pagina a la
        navegacion.
      </Typography.Paragraph>

      <Alert
        type="info"
        showIcon
        title="Flujo listo para continuar trabajando desde la bandeja"
        description="El usuario puede revisar este detalle contextual y volver al listado desde la accion visible del panel, manteniendo el patron master-detail del modulo."
      />

      <Card title="Bloques previstos del detalle">
        <Typography.Paragraph>
          Aqui se reservara espacio para informacion del documento, acciones del
          usuario, historial, adjuntos y confirmaciones de envio sin romper la
          navegacion principal.
        </Typography.Paragraph>
        <Divider />
        <Typography.Text type="secondary">
          Version inicial sin integracion backend ni logica funcional de
          respuesta.
        </Typography.Text>
      </Card>
    </Space>
  );
}
