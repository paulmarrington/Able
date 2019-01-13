namespace Decoupled.Mock {
  using UnityEngine;

  /// <a href="http://bit.ly/2APZMoL">MonoBehaviour that when active in a scene will replace services with a mock that will run without a real-world device</a> <inheritdoc />
  public class Mock<T> : MonoBehaviour where T : Service, new() {
    /// <a href="http://bit.ly/2APZMoL">An instance of the Mock service created here and used to replace any default service</a>
    protected T MockService;

    /// <a href="http://bit.ly/2APZMoL">Register the mock as the service to use for this hardware</a>
    protected virtual void Awake() => Service.RegisterAsMock(MockService = new T { Name = GetType().Name });
  }

  // // // TEMPLATE FOR MOCK SERVICES (must be in file of MockTemplate name) // // //

  /// <a href="http://bit.ly/2APZMoL">Sample mono-behaviour template for mocked service</a> <inheritdoc />
  public class MockTemplate : Mock<MockTemplate.Service> {
    // Data that can be changed in the inspector to effect mock results
    // ReSharper disable once NotAccessedField.Local
    [SerializeField] private string templateData;

    /// <a href="http://bit.ly/2APZMoL">Implement mock service here</a> <inheritdoc />
    public class Service : TemplateService { /* mock implementations of service methods */
    }
  }

  /// <a href="http://bit.ly/2APZMoL">Sample service interface</a><inheritdoc />
  public class TemplateService : Service<TemplateService> { /* abstract or default service methods */
  }
}