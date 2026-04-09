## Purpose

Definir el contrato funcional y las reglas de comportamiento del componente AppTabs como wrapper enterprise de Ant Design Tabs, con tipado estricto y control claro de estado.

## Requirements

### Requirement: Wrapper controlado con contrato estricto
AppTabs SHALL operar como wrapper de AntD Tabs con contrato tipado estricto y sin `any`.

#### Scenario: Tipado estricto
- **WHEN** se consumen props de AppTabs
- **THEN** el tipado expone solo API permitida y extiende `ComponentProps<typeof Tabs>`

### Requirement: Controlado vs no controlado
AppTabs SHALL soportar `activeKey` (controlado) y `defaultActiveKey` (no controlado) sin mezclar.

#### Scenario: activeKey domina
- **WHEN** se provee `activeKey`
- **THEN** `defaultActiveKey` es ignorado

### Requirement: Bloqueo por disabled y beforeChange
AppTabs SHALL bloquear cambios si el tab destino esta `disabled` o `beforeChange` retorna false.

#### Scenario: Tab disabled
- **WHEN** se intenta cambiar a un tab con `disabled: true`
- **THEN** no se ejecuta `onChange` ni se cambia `activeKey`

### Requirement: Mapper interno
AppTabs SHALL mapear items mediante `mapToAntdItems(items)` sin mutar el arreglo original.

#### Scenario: Mapper estable
- **WHEN** se construyen items para AntD
- **THEN** no se muta `items` y se respeta icono/badge/label
