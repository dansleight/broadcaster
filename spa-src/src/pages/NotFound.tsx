import { Col, Container, Row } from "react-bootstrap";
import { Headcrumb } from "../components/Headcrumb";
import { useIdentityContext } from "../contexts/UseContexts";

export function NotFound() {
  const { user } = useIdentityContext();

  return (
    <Container fluid>
      <Headcrumb title="Not Found" />
      <Row>
        <Col>
          <dl>
            <dt>Name</dt>
            <dd>{user.name}</dd>

            <dt>Email</dt>
            <dd>{user.email}</dd>
          </dl>
        </Col>
      </Row>
    </Container>
  );
}
