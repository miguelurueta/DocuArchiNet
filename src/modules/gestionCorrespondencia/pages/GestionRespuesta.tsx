import { Alert, Card, Divider, Space, Typography } from "antd";

export default function GestionRespuesta() {
  return (
    <Space orientation="vertical" size={16} style={{ display: "flex" }}>
      <Typography.Title level={4} style={{ margin: 0 }}>
        Gestion de respuesta
      </Typography.Title>
      <Typography.Paragraph style={{ margin: 0 }}>
        Esta vista secundaria funciona como placeholder del flujo de respuesta
        contextual. Su renderizacion depende de la ruta hija y se presenta
        dentro del panel persistente del modulo.
      </Typography.Paragraph>

      <Alert
        type="info"
        showIcon
        title="Area preparada para futuras iteraciones"
        description="Aqui se integraran formularios, validaciones y reglas del dominio cuando el backlog del modulo avance."
      />

      <Card title="Bloques previstos">
        <Typography.Paragraph>
          Se reservara espacio para informacion del documento, acciones del
          usuario, historial y confirmaciones de envio.
        </Typography.Paragraph>
        <Divider />
        <Typography.Text type="secondary">
          Version inicial sin integracion backend ni logica funcional.
        </Typography.Text>
      </Card>
    </Space>
  );
}
