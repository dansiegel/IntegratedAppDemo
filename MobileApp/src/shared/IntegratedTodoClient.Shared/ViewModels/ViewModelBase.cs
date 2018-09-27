using System;
using System.Linq;
using System.Threading.Tasks;
using Prism.Mvvm;
using Prism;
using Prism.AppModel;
using Prism.Navigation;
using Prism.Services;
using IntegratedTodoClient.Services;
using Prism.Logging;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace IntegratedTodoClient.ViewModels
{
    public class ViewModelBase : BindableBase, IActiveAware, INavigationAware, IDestructible, IConfirmNavigation, IConfirmNavigationAsync, IApplicationLifecycleAware, IPageLifecycleAware
    {
        protected IPageDialogService _pageDialogService { get; }

        protected INavigationService _navigationService { get; }

        protected ILogger _logger { get; }

        public ViewModelBase(INavigationService navigationService, IPageDialogService pageDialogService, ILogger logger)
        {
            _logger = logger;
            _pageDialogService = pageDialogService;
            _navigationService = navigationService;
        }

        public string Title { get; set; }

        public string Subtitle { get; set; }

        public string Icon { get; set; }

        public bool IsBusy { get; set; }

        public bool IsNotBusy { get; set; }

        public bool CanLoadMore { get; set; }

        public string Header { get; set; }

        public string Footer { get; set; }

        private void OnIsBusyChanged() => IsNotBusy = !IsBusy;

        private void OnIsNotBusyChanged() => IsBusy = !IsNotBusy;

        protected virtual Task HandleNavigationRequestAsync(string uri) => HandleNavigationRequestAsync(uri, null);

        protected virtual async Task HandleNavigationRequestAsync(string uri, INavigationParameters parameters)
        {
            try
            {
                await _navigationService.NavigateAsync(uri, parameters);
            }
            catch (Exception ex)
            {
                var correlationId = Guid.NewGuid().ToString();
                var report = new Dictionary<string, string>
                {
                    { "RequestUri", uri },
                    { "CorrelationId", correlationId }
                };
                foreach(var param in parameters)
                {
                    if(param.Value.GetType().IsPrimitive)
                    {
                        report.Add(param.Key, param.Value.ToString());
                    }
                    else
                    {
                        report.Add(param.Key, JsonConvert.SerializeObject(param.Value));
                    }
                }
                _logger.Report(ex, report);
                await _pageDialogService.DisplayAlertAsync("Error", $"An unexpected error occured '{ex.GetType().Name}'. Please provide support this Correlation Id {correlationId}", "Ok");
            }
        }

        #region IActiveAware

        public bool IsActive { get; set; }

        public event EventHandler IsActiveChanged;

        private void OnIsActiveChanged()
        {
            IsActiveChanged?.Invoke(this, EventArgs.Empty);

            if (IsActive)
            {
                OnIsActive();
            }
            else
            {
                OnIsNotActive();
            }
        }

        protected virtual void OnIsActive() { }

        protected virtual void OnIsNotActive() { }

        #endregion IActiveAware

        #region INavigationAware

        public virtual void OnNavigatingTo(INavigationParameters parameters) { }

        public virtual void OnNavigatedTo(INavigationParameters parameters) { }

        public virtual void OnNavigatedFrom(INavigationParameters parameters) { }

        #endregion INavigationAware

        #region IDestructible

        public virtual void Destroy() { }

        #endregion IDestructible

        #region IConfirmNavigation

        public virtual bool CanNavigate(INavigationParameters parameters) => true;

        public virtual Task<bool> CanNavigateAsync(INavigationParameters parameters) =>
            Task.FromResult(CanNavigate(parameters));

        #endregion IConfirmNavigation

        #region IApplicationLifecycleAware

        public virtual void OnResume() { }

        public virtual void OnSleep() { }

        #endregion IApplicationLifecycleAware

        #region IPageLifecycleAware

        public virtual void OnAppearing() { }

        public virtual void OnDisappearing() { }

        #endregion IPageLifecycleAware
    }
}