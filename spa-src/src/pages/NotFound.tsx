import { Col, Container, Row } from "react-bootstrap";
import { Headcrumb } from "../components/Headcrumb";
import { useIdentityContext } from "../contexts/UseContexts";

export function NotFound() {
  const { name, username } = useIdentityContext();

  return (
    <Container fluid>
      <Headcrumb title="Not Found" />
      <Row>
        <Col>
          <dl>
            <dt>Name</dt>
            <dd>{name}</dd>

            <dt>User Name</dt>
            <dd>{username}</dd>
          </dl>
        </Col>
      </Row>
    </Container>
  );
}
