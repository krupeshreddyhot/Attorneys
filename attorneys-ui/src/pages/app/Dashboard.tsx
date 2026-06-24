import { useEffect, useState } from "react";
import { Card, CardContent, Grid, Typography } from "@mui/material";
import { Navigate } from "react-router-dom";
import api from "../../services/api";
import { useAuth } from "../../context/AuthContext";
import PageHeader from "../../components/layout/PageHeader";

type Summary = {
  totalCases: number;
  totalCourts: number;
  todaysHearings: number;
  totalAccounts: number;
};

const Dashboard = () => {
  const { isSuperAdmin } = useAuth();
  const [summary, setSummary] = useState<Summary | null>(null);

  useEffect(() => {
    if (isSuperAdmin) return;
    api.get<Summary>("/dashboard/summary").then((res) => setSummary(res.data)).catch(() => undefined);
  }, [isSuperAdmin]);

  if (isSuperAdmin) return <Navigate to="/app/organizations" replace />;

  const cards = [
    { label: "Total Cases", value: summary?.totalCases ?? "—" },
    { label: "Courts", value: summary?.totalCourts ?? "—" },
    { label: "Today's Hearings", value: summary?.todaysHearings ?? "—" },
    { label: "Accounts", value: summary?.totalAccounts ?? "—" },
  ];

  return (
    <>
      <PageHeader title="Dashboard" />
      <Grid container spacing={2}>
        {cards.map((card) => (
          <Grid key={card.label} size={{ xs: 6, sm: 6, md: 3 }}>
            <Card sx={{ height: "100%" }}>
              <CardContent sx={{ p: { xs: 2, sm: 3 } }}>
                <Typography color="text.secondary" variant="body2">
                  {card.label}
                </Typography>
                <Typography variant="h4" sx={{ fontWeight: 700, fontSize: { xs: "1.75rem", sm: "2.125rem" } }}>
                  {card.value}
                </Typography>
              </CardContent>
            </Card>
          </Grid>
        ))}
      </Grid>
    </>
  );
};

export default Dashboard;
