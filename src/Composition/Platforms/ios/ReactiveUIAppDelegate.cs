using System;
using System.Linq;
using System.Reflection;
using Autofac;
using ReactiveUI;
using UIKit;

namespace Composition
{
    /// <summary>
    /// ReactiveUI <see cref="UIApplicationDelegate"/> abstraction.
    /// </summary>
    /// <seealso cref="UIKit.UIApplicationDelegate" />
    public abstract class ReactiveUIAppDelegate : UIApplicationDelegate
    {
        /// <summary>
        /// Gets or sets the container where application registrations exist.
        /// </summary>
        public virtual IContainer Container { get; protected set; }

        /// <summary>
        /// Sets the target navigation page for application startup.
        /// </summary>
        /// <typeparam name="TViewModel">The view model type.</typeparam>
        public abstract void NavigateToStart<TViewModel>();

        /// <summary>
        /// Registers custom command binders.
        /// </summary>
        /// <param name="builder">The container builder.</param>
        public virtual void RegisterCommandBinders(ContainerBuilder builder)
        {
        }

        /// <summary>
        /// Registers application dependencies.
        /// </summary>
        /// <param name="builder">The container builder.</param>
        public virtual void RegisterDependencies(ContainerBuilder builder)
        {
        }

        /// <summary>
        /// Registers Views to View Models.
        /// </summary>
        /// <param name="builder">The container builder.</param>
        public virtual void RegisterViews(ContainerBuilder builder)
        {
        }

        /// <summary>
        /// Registers implementations.
        /// </summary>
        /// <param name="builder">The container builder.</param>
        public virtual void RegisterViewModels(ContainerBuilder builder) =>
            builder.RegisterAssemblyTypes(AppDomain.CurrentDomain.GetAssemblies())
                .Where(t => t.GetTypeInfo().Name.Contains("ViewModel") &&
                            t.GetTypeInfo().ImplementedInterfaces.All(x => x != typeof(IScreen)))
                .AsSelf()
                .AsImplementedInterfaces();

        /// <summary>
        /// Composes a <see cref="UIApplicationDelegate"/> for iOS start up.
        /// </summary>
        /// <param name="builder">The container builder.</param>
        public void ComposeApplicationDelegate(ContainerBuilder builder)
        {
            RegisterViews(builder);
            RegisterViewModels(builder);
            RegisterDependencies(builder);
            RegisterCommandBinders(builder);
            Container = builder.Build();
        }

        /// <summary>
        /// Releases the resources used by the NSObject object.
        /// </summary>
        /// <remarks>
        /// <para>The Dispose method releases the resources used by the NSObject class.</para>
        /// <para>Calling the Dispose method when the application is finished using the NSObject ensures that all external resources used by this managed object are released as soon as possible.  Once developers have invoked the Dispose method, the object is no longer useful and developers should no longer make any calls to it.  For more information on releasing resources see ``Cleaning up Unmananaged Resources'' at http://msdn.microsoft.com/en-us/library/498928w2.aspx.</para>
        /// </remarks>
        public new void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Releases the resources used by the NSObject object.
        /// </summary>
        /// <param name="disposing">If set to <see langword="true" />, the method is invoked directly and will dispose manage and unmanaged resources;   If set to <see langword="false" /> the method is being called by the garbage collector finalizer and should only release unmanaged resources.</param>
        /// <remarks>
        /// <para>This Dispose method releases the resources used by the NSObject class.</para>
        /// <para>This method is called by both the Dispose() method and the object finalizer (Finalize).    When invoked by the Dispose method, the parameter disposing <paramref name="disposing" /> is set to <see langword="true" /> and any managed object references that this object holds are also disposed or released;  when invoked by the object finalizer, on the finalizer thread the value is set to <see langword="false" />. </para>
        /// <para>Calling the Dispose method when the application is finished using the NSObject ensures that all external resources used by this managed object are released as soon as possible.  Once developers have invoked the Dispose method, the object is no longer useful and developers should no longer make any calls to it.</para>
        /// <para>  For more information on how to override this method and on the Dispose/IDisposable pattern, read the ``Implementing a Dispose Method'' document at http://msdn.microsoft.com/en-us/library/fs2xkftw.aspx.</para>
        /// </remarks>
        protected void Dispose(bool disposing)
        {
            if (disposing)
            {
                // free managed resources
                Container?.Dispose();
                Container = null;
            }
        }
    }
}
