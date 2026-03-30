# AppContent

## Proposito

`AppContent` es un componente reusable de la capa UI compartida para encapsular el contenedor principal de contenido de una vista con estructura consistente de `header`, `body` y `footer`, sin acoplar a los modulos consumidores a wrappers ad hoc o a `Layout.Content`.

Su primera adopcion real esta en el modulo de gestion de correspondencia y el contrato funcional esta respaldado por el cambio OpenSpec `scrumcore-13-crear-componente-content`.

## Ubicacion

- Implementacion: `src/app/Components/UI/AppContent/AppContent.tsx`
- Estilos: `src/app/Components/UI/AppContent/AppContent.module.css`
- Tests: `src/app/Components/UI/AppContent/AppContent.test.tsx`
- Export: `src/app/Components/UI/AppContent/index.ts`

## API publica

### `AppContentProps`

- `as?: "section" | "article" | "div" | "main"`
  Permite cambiar el elemento semantico raiz del contenedor.
- `children: ReactNode`
  Contenido principal obligatorio del cuerpo.
- `header?: ReactNode`
  Region superior opcional para encabezados, toolbars o contexto auxiliar.
- `footer?: ReactNode`
  Region inferior opcional para acciones o resumenes finales.
- `className?: string`
  Clase CSS opcional aplicada al contenedor raiz.
- `contentClassName?: string`
  Clase CSS opcional aplicada unicamente al bloque del cuerpo.
- `width?: "default" | "wide" | "full"`
  Variante controlada para el ancho util del contenedor.
- `density?: "comfortable" | "compact"`
  Variante controlada para el espaciado interno.

Ademas, `AppContent` propaga los props nativos compatibles del elemento raiz seleccionado mediante `as`.

## Ejemplo de uso

```tsx
import { Card, Col, Row, Space, Tag, Typography } from "antd";
import { useNavigate } from "react-router-dom";
import { AppContent } from "../../../app/Components/UI/AppContent";
import { AppToolbar } from "../../../app/Components/UI/AppToolbar";

export default function GestionCorrespondencia() {
  const navigate = useNavigate();

  return (
    <>
      <AppToolbar
        title="Centro operativo del modulo"
        subtitle="AppToolbar enterprise"
        description="Vista base para incorporar bandejas, detalle y acciones de respuesta."
        breadcrumbs={[
          { key: "dashboard", label: "Dashboard", to: "/dashboard" },
          { key: "gestion-correspondencia", label: "Gestion de correspondencia", current: true },
        ]}
        extra={
          <Space wrap>
            <Tag color="blue">React Router anidado</Tag>
            <Tag color="cyan">Ant Design</Tag>
          </Space>
        }
        primaryAction={{
          key: "open-response",
          label: "Abrir respuesta contextual",
          variant: "primary",
          onClick: () => navigate("respuesta"),
        }}
      />

      <AppContent width="wide">
        <Row gutter={[16, 16]}>
          <Col xs={24} md={8}>
            <Card title="Bandeja prioritaria">
              <Typography.Paragraph style={{ marginBottom: 0 }}>
                Resumen para documentos pendientes de clasificacion y respuesta.
              </Typography.Paragraph>
            </Card>
          </Col>
        </Row>
      </AppContent>
    </>
  );
}
```

## Comportamiento responsive

- El componente centraliza el ancho maximo del contenido y lo adapta mediante la variante `width`.
- La variante `density` controla el padding interno sin requerir estilos inline arbitrarios.
- En pantallas reducidas disminuye el radio y mantiene el contenido a ancho completo para evitar desbordamientos evitables.
- `header`, `body` y `footer` comparten una misma base visual para preservar consistencia entre vistas.

## Accesibilidad

- `AppContent` permite elegir el elemento raiz semantico con `as` para ajustarse al contexto de la pagina.
- Las regiones opcionales solo se renderizan cuando existen, evitando estructura vacia innecesaria.
- La composicion de `header` y `footer` permite integrar componentes accesibles del proyecto sin alterar su semantica.

## Cobertura de pruebas

Se validan al menos estos escenarios:

- renderizado del contenido principal sin regiones opcionales
- renderizado de `header` y `footer` cuando se suministran
- aplicacion de variantes de ancho, densidad y clases adicionales
- cambio del elemento semantico raiz
- integracion del componente en `gestionCorrespondencia`

## Notas

- El barrel `src/app/Components/UI/index.ts` exporta `AppContent`, pero en tests puede ser preferible importar desde `src/app/Components/UI/AppContent` para evitar efectos colaterales de otros exports compartidos.
- La primera version mantiene una API controlada: no expone props de estilo arbitrarios ni layout dinamico fuera de `width`, `density`, `className` y `contentClassName`.
- En `GestionCorrespondencia`, `AppContent` se usa como contenedor principal del bloque operativo, separado de `AppToolbar`; no debe absorber el toolbar en su region `header` para ese caso de uso.
