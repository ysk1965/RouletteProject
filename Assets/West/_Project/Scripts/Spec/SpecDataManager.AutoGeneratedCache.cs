//=====================================================
//  자동 생성 코드입니다. 수정하지 마세요
//  made by 김윤하, mail : yhkim2@cookapps.com 
//=====================================================
using System.Collections.Generic;
using System.Linq;
using CookApps.BM.MVPWest;
	public partial class SpecDataManager
	{
		public List<SpecGameConfig> SpecGameConfigList {get; private set;}
		public List<SpecLanguage> SpecLanguageList {get; private set;}
		public List<SpecCharacter> SpecCharacterList {get; private set;}
		public List<SpecPerk> SpecPerkList {get; private set;}
		public List<SpecRule> SpecRuleList {get; private set;}
		public List<SpecRuleScenario> SpecRuleScenarioList {get; private set;}
		public List<SpecItem> SpecItemList {get; private set;}
		public List<SpecGameItem> SpecGameItemList {get; private set;}
		public List<SpecStage> SpecStageList {get; private set;}

		private void GenerateCacheSpecData()
		{
			SpecGameConfigList = SpecData.SpecGameConfig.All.ToList();
			SpecLanguageList = SpecData.SpecLanguage.All.ToList();
			SpecCharacterList = SpecData.SpecCharacter.All.ToList();
			SpecPerkList = SpecData.SpecPerk.All.ToList();
			SpecRuleList = SpecData.SpecRule.All.ToList();
			SpecRuleScenarioList = SpecData.SpecRuleScenario.All.ToList();
			SpecItemList = SpecData.SpecItem.All.ToList();
			SpecGameItemList = SpecData.SpecGameItem.All.ToList();
			SpecStageList = SpecData.SpecStage.All.ToList();
		}
	}
