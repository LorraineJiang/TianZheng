using UnityEngine;
using System.Globalization;
using System.Collections;
using Loxodon.Framework.Contexts;
using Loxodon.Framework.Views;
using Loxodon.Log;
using Loxodon.Framework.Binding;
using Loxodon.Framework.Localizations;
using Loxodon.Framework.Services;


[RequireComponent(typeof(GlobalWindowManager))]
[DisallowMultipleComponent]
public class Launcher : MonoBehaviour
{
	private ApplicationContext context;
	private WindowContainer winContainer;
	void Awake()
	{
		GlobalWindowManager windowManager = FindObjectOfType<GlobalWindowManager>();
		if (windowManager == null)
			throw new NotFoundException("Not found the GlobalWindowManager.");

		context = Context.GetApplicationContext();

		IServiceContainer container = context.GetContainer();

		/* Initialize the data binding service */
		BindingServiceBundle bundle = new BindingServiceBundle(context.GetContainer());
		bundle.Start();

		///* 初始化IUIViewLocator，并注册到容器中 */
		container.Register<IUIViewLocator>(new ResourcesViewLocator());

		///* Initialize the localization service */
		////CultureInfo cultureInfo = Locale.GetCultureInfoByLanguage (SystemLanguage.English);
		//CultureInfo:使同一个数据适应不同地区和文化
		CultureInfo cultureInfo = Locale.GetCultureInfo();
		var localization = Localization.Current;
		localization.CultureInfo = cultureInfo;
		localization.AddDataProvider(new DefaultDataProvider("Localization", new XmlDocumentParser()));

		///* register Localization */
		container.Register<Localization>(localization);

		///* register AccountRepository */
		//IAccountRepository accountRepository = new AccountRepository();
		//container.Register<IAccountService>(new AccountService(accountRepository));
		DontDestroyOnLoad(this);
	}

	IEnumerator Start()
	{
		/* Create a window container */
		winContainer = WindowContainer.Create("MAIN");
		LoxodonWindowCtr.WindowContainer = winContainer;
		LoxodonWindowCtr.AppContext = context;	//设置全局上下文

		yield return null;
		//在Assert的Resources文件夹中找到prefab_LoginWindow所表示的预制体并在Win容器中打开
		LoxodonWindowCtr.Instance.OpenWindow<LoginWindow>(Global.prefab_LoginWindow);

		//yield return transition.WaitForDone();
	}
}
