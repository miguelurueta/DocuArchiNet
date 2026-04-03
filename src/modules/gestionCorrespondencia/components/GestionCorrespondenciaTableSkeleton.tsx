import { Skeleton, Stack } from "@mui/material";

export default function GestionCorrespondenciaTableSkeleton() {
  return (
    <Stack spacing={1.5} data-testid="gestion-correspondencia-skeleton">
      <Skeleton variant="rounded" height={56} />
      <Skeleton variant="rounded" height={48} />
      <Skeleton variant="rounded" height={40} />
      <Skeleton variant="rounded" height={320} />
    </Stack>
  );
}
