import {
  Avatar,
  Box,
  Button,
  Card,
  CardContent,
  CircularProgress,
  Container,
  Grid,
  Stack,
  Typography,
} from "@mui/material";
import GavelIcon from "@mui/icons-material/Gavel";
import { Link as RouterLink, Navigate } from "react-router-dom";
import HeroCarousel from "../../components/landing/HeroCarousel";
import { useFirmLanding } from "../../context/FirmLandingContext";
import { publicMediaUrl } from "../../utils/mediaUrl";

const pagePadding = { px: { xs: 2, sm: 3, md: 4 } };

const FirmLandingPage = () => {
  const { firmCode, landing, loading, notFound } = useFirmLanding();

  if (notFound) return <Navigate to="/" replace />;
  if (loading || !landing) {
    return (
      <Box sx={{ display: "flex", justifyContent: "center", py: 12 }}>
        <CircularProgress sx={{ color: "#c9a227" }} />
      </Box>
    );
  }

  const loginPath = `/login?firm=${landing.code}`;
  const contactLine = [landing.addressLine, landing.city].filter(Boolean).join(", ");

  return (
    <>
      <HeroCarousel
        slides={landing.banners.map((b) => ({ id: b.id, imageUrl: b.imageUrl, caption: b.caption }))}
        overlay={
          <>
            <Typography variant="overline" sx={{ color: "#c9a227", letterSpacing: 2 }}>
              Advocates & Legal Consultants
            </Typography>
            <Typography
              variant="h2"
              sx={{ mt: 1, maxWidth: 720, fontWeight: 700, fontSize: { xs: "1.75rem", sm: "2.5rem", md: "3.75rem" }, lineHeight: 1.15 }}
            >
              {landing.heroTagline ?? "We provide legal help for your cases"}
            </Typography>
            <Typography variant="h6" sx={{ mt: 2, maxWidth: 640, opacity: 0.9, fontSize: { xs: "1rem", sm: "1.25rem" } }}>
              {landing.heroSubtitle ??
                "Trusted chamber for court case management, diary tracking, and client representation."}
            </Typography>
            <Stack direction={{ xs: "column", sm: "row" }} spacing={2} sx={{ mt: 4 }}>
              <Button
                component={RouterLink}
                to={loginPath}
                variant="contained"
                size="large"
                sx={{ width: { xs: "100%", sm: "auto" }, bgcolor: "#c9a227", color: "#0d1b2a", "&:hover": { bgcolor: "#b8921f" } }}
              >
                Staff Login
              </Button>
              <Button
                component={RouterLink}
                to={`/${firmCode}#contact`}
                variant="outlined"
                size="large"
                sx={{ width: { xs: "100%", sm: "auto" }, borderColor: "#c9a227", color: "#c9a227" }}
              >
                Contact Us
              </Button>
            </Stack>
          </>
        }
      />

      <Box id="about" sx={{ py: { xs: 5, sm: 8 } }}>
        <Container maxWidth={false} sx={pagePadding}>
          <Grid container spacing={4} sx={{ alignItems: "center" }}>
            <Grid size={{ xs: 12, md: 6 }}>
              <Typography variant="overline" color="primary">
                About
              </Typography>
              <Typography variant="h4" sx={{ fontWeight: 700, fontSize: { xs: "1.5rem", sm: "2.125rem" } }} gutterBottom>
                {landing.aboutTitle ?? landing.name}
              </Typography>
              <Typography color="text.secondary" sx={{ mb: 2 }}>
                {landing.aboutBody ?? "Welcome to our law chamber."}
              </Typography>
            </Grid>
            <Grid size={{ xs: 12, md: 6 }}>
              <Card elevation={0} sx={{ bgcolor: "#f8f9fa", borderLeft: "4px solid #c9a227" }}>
                <CardContent>
                  <Typography variant="h6" gutterBottom>
                    {landing.aboutHighlightTitle ?? "Experience · Trust · Results"}
                  </Typography>
                  <Typography color="text.secondary">
                    {landing.aboutHighlightBody ??
                      "From case filing to next-date diary and payment tracking, manage your practice in one secure application."}
                  </Typography>
                </CardContent>
              </Card>
            </Grid>
          </Grid>
        </Container>
      </Box>

      {landing.practiceAreas.length > 0 && (
        <Box id="practice" sx={{ bgcolor: "#f4f6f8", py: { xs: 5, sm: 8 } }}>
          <Container maxWidth={false} sx={pagePadding}>
            <Typography variant="h4" sx={{ fontWeight: 700, textAlign: "center", fontSize: { xs: "1.5rem", sm: "2.125rem" } }} gutterBottom>
              Practice Areas
            </Typography>
            <Typography sx={{ textAlign: "center", color: "text.secondary", mb: 4 }}>
              Areas of law we handle
            </Typography>
            <Grid container spacing={3}>
              {landing.practiceAreas.map((area) => (
                <Grid key={area.id} size={{ xs: 12, sm: 6, md: 3 }}>
                  <Card sx={{ height: "100%", textAlign: "center", p: 2 }}>
                    <CardContent>
                      <GavelIcon fontSize="large" sx={{ color: "#c9a227" }} />
                      <Typography variant="h6" sx={{ mt: 2 }}>
                        {area.title}
                      </Typography>
                      {area.description && (
                        <Typography variant="body2" color="text.secondary" sx={{ mt: 1 }}>
                          {area.description}
                        </Typography>
                      )}
                    </CardContent>
                  </Card>
                </Grid>
              ))}
            </Grid>
          </Container>
        </Box>
      )}

      {landing.advocates.length > 0 && (
        <Box id="advocates" sx={{ py: { xs: 5, sm: 8 } }}>
          <Container maxWidth={false} sx={pagePadding}>
            <Typography variant="h4" sx={{ fontWeight: 700, textAlign: "center", fontSize: { xs: "1.5rem", sm: "2.125rem" } }} gutterBottom>
              Our Advocates
            </Typography>
            <Typography sx={{ textAlign: "center", color: "text.secondary", mb: 4 }}>
              Meet the legal team at {landing.name}
            </Typography>
            <Box sx={{ display: "flex", flexWrap: "wrap", gap: 3, justifyContent: "center" }}>
              {landing.advocates.map((advocate) => (
                <Card key={advocate.id} sx={{ width: { xs: "100%", sm: 340 }, textAlign: "center", p: 3 }}>
                  <CardContent>
                    <Avatar
                      src={advocate.photoUrl ? publicMediaUrl(advocate.photoUrl) : undefined}
                      sx={{ width: 96, height: 96, mx: "auto", bgcolor: "#0d1b2a" }}
                    >
                      {advocate.fullName.charAt(0)}
                    </Avatar>
                    <Typography variant="h6" sx={{ mt: 2, fontWeight: 700 }}>
                      {advocate.fullName}
                    </Typography>
                    {advocate.designation && (
                      <Typography variant="body2" sx={{ color: "#c9a227", fontWeight: 600 }}>
                        {advocate.designation}
                      </Typography>
                    )}
                    {advocate.bio && (
                      <Typography variant="body2" color="text.secondary" sx={{ mt: 1.5 }}>
                        {advocate.bio}
                      </Typography>
                    )}
                  </CardContent>
                </Card>
              ))}
            </Box>
          </Container>
        </Box>
      )}

      <Box sx={{ py: { xs: 5, sm: 8 }, textAlign: "center" }}>
        <Container maxWidth={false} sx={pagePadding}>
          <Typography variant="h5" sx={{ fontWeight: 700 }}>
            Free Case Consultation
          </Typography>
          <Typography color="text.secondary" sx={{ mt: 1 }}>
            Contact the chamber for an appointment
          </Typography>
          <Button component={RouterLink} to={loginPath} variant="contained" sx={{ mt: 3, bgcolor: "#0d1b2a" }}>
            Access Case System
          </Button>
        </Container>
      </Box>

      <Box id="contact" component="footer" sx={{ bgcolor: "#0d1b2a", color: "#fff", py: 6 }}>
        <Container maxWidth={false} sx={pagePadding}>
          <Typography variant="h5" sx={{ fontWeight: 700 }}>
            Contact
          </Typography>
          {contactLine && <Typography sx={{ mt: 2, opacity: 0.85 }}>{contactLine}</Typography>}
          {landing.phone && <Typography sx={{ opacity: 0.85 }}>Phone: {landing.phone}</Typography>}
          {landing.email && <Typography sx={{ opacity: 0.85 }}>Email: {landing.email}</Typography>}
          <Typography variant="body2" sx={{ mt: 4, opacity: 0.6 }}>
            © {new Date().getFullYear()} {landing.name}. Legal practice management.
          </Typography>
        </Container>
      </Box>
    </>
  );
};

export default FirmLandingPage;
